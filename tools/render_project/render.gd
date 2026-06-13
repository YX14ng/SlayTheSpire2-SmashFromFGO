extends Node3D
# Renders FGO servant FBX animations to PNG frame sequences for the StS2 mod.
# Two-pass flow to avoid baked-in clipping:
#   PASS = "measure": renders selected frames, accumulates the union content rect
#                     across forms into crop_union.txt (no PNGs written).
#   PASS = "save":    renders selected frames cropped to the merged union rect.
# Run once per form in measure mode, then once per form in save mode.

const PASS := "save"   # "measure" | "save" | "list" (lista clips) | "probe" (movimiento por frame)
const FPS := 30
const OUT_DIR := "res://frames"
const CROP_FILE := "res://crop_union.txt"
# 1024px canvas at double the old camera size = same pixel density, 2x margin.
const CAM_SIZE := 43.8

# Selected frame windows per model: anim -> [from, to, step] (indices at 30fps).
const SELECT := {
	"800100": { "idle": [0, 155, 2], "attack": [27, 53, 1], "cast": [0, 59, 2], "hurt": [0, 16, 1] },
	"800150": { "idle": [0, 154, 2], "attack": [0, 20, 1], "cast": [0, 79, 2], "hurt": [0, 16, 1] },
	"800200": { "idle": [0, 156, 2], "attack": [20, 66, 1], "cast": [0, 64, 2], "hurt": [0, 16, 1] },
	# Morgan: los spell invocan el espejo gigante (llena el canvas) — cast recortado
	# al gesto previo a que aparezca el prop. Idle a 30fps (step 1): a 15fps el pelo
	# y la capa se ven entrecortados (feedback de playtest).
	"704020": { "idle": [0, 153, 1], "attack": [48, 68, 1], "cast": [0, 5, 1], "hurt": [0, 16, 1] },
	"505320": { "idle": [0, 153, 1], "attack": [28, 48, 1], "cast": [0, 7, 1], "hurt": [0, 16, 1] },
	"704030": { "idle": [0, 153, 1], "attack": [48, 68, 1], "cast": [0, 5, 1], "hurt": [0, 16, 1] },
	# Artoria: Castoria (504520, clips *_level_3) y las dos formas Berserker de
	# verano (704710 lv2 / 704720 lv3). attack de 704710 = attack_q (override):
	# attack_b/attack_a son surfs aereos con root motion salvaje. cast de 704720 =
	# el final del spell [76..96]: el resto invoca el piano de hielo gigante
	# (mismo caso que el espejo de Morgan). Idle a step 1 (30fps, regla Morgan).
	"504520": { "idle": [0, 153, 1], "attack": [28, 68, 1], "cast": [0, 72, 2], "hurt": [0, 16, 1] },
	"704710": { "idle": [0, 154, 1], "attack": [3, 33, 1], "cast": [0, 74, 2], "hurt": [0, 16, 1] },
	"704720": { "idle": [0, 154, 1], "attack": [20, 62, 1], "cast": [76, 96, 1], "hurt": [0, 16, 1] },
}
const CLIP_FOR := { "idle": "wait", "attack": "attack_b", "cast": "spell", "hurt": "damage_01" }
# Overrides de clip por modelo: el attack_q de la Berserker de verano es el
# unico ataque a nivel de piso (lanza las espadas telequineticamente + patada
# acrobatica); attack_b y attack_a son surfs aereos sobre la espada.
const CLIP_OVERRIDE := { "704710": { "attack": "attack_q" } }
# Anims excluidas del union de crop: el attack_b de Aesc lanza fragmentos de corona
# hasta el borde del canvas; en el save esos fragmentos se recortan y la figura queda.
# El attack_q de 704710 lanza las espadas telequineticas por TODO el canvas
# (union 2048x2048 = recorte muerto) — mismo tratamiento: las espadas se cortan
# en el borde del crop durante el ataque, la figura queda intacta.
const MEASURE_SKIP := { "505320": ["attack"], "704710": ["attack"] }
# Mallas de props ocultadas por modelo (patrones, match por contiene).
const HIDE_MESHES := {}
# Huesos colapsados a escala 0 en cada pose (mismo truco que FACE_POSE): las 4
# espadas teal del NP de Castoria viven DENTRO de la malla weapon (no se pueden
# ocultar por nodo) colgadas de joint_weaponA-D; en el juego solo aparecen en el
# NP (clips treasureArms). joint_sword es el baculo — NO tocarlo.
const HIDE_BONES := { "504520": ["joint_weaponA", "joint_weaponB", "joint_weaponC", "joint_weaponD"] }

const FACE_POSE := {
	"joint_open_eye": 1.0, "joint_close_eye": 0.0,
	"joint_open_mouth": 0.0, "joint_close_mouth": 1.0,
	"joint_eyebrow": 1.0, "joint_eyebrow_attack": 0.0,
}
# Lista EXPLICITA de frames por modelo/anim (gana sobre la ventana [from,to,step] de
# SELECT). Las Berserker de verano (704710/704720) tienen un idle (wait) que agacha la
# cabeza ~94% del ciclo => ojos tapados por el flequillo "la mayor parte del tiempo"
# (los ojos renderizan perfecto, el problema es la pose). La cabeza solo esta arriba en
# la costura del loop (154->0, que ES el punto de loop natural del clip). Re-ventaneamos
# el idle a ese tramo cabeza-arriba: la secuencia se guarda 000,001,... en ESTE orden y
# loopea sin salto porque cruza 154->0. El save usa contador secuencial (no el indice).
const FRAMES_OVERRIDE := {
	"704710": { "idle": [150, 151, 152, 153, 154, 0, 1, 2, 3, 4] },
	"704720": { "idle": [150, 151, 152, 153, 154, 0, 1, 2, 3, 4] },
}

var _model: Node3D
var _player: AnimationPlayer
var _skeleton: Skeleton3D
var _model_id := ""

func _ready() -> void:
	get_viewport().transparent_bg = true

	for f in DirAccess.get_files_at("res://"):
		if f.ends_with(".png") and not f.begins_with("debug"):
			_model_id = f.get_basename()
			break
	print("MODEL: ", _model_id, "  PASS: ", PASS)

	var packed: PackedScene = load("res://chr.fbx")
	_model = packed.instantiate()
	add_child(_model)

	_player = _model.find_child("AnimationPlayer", true, false)

	if PASS == "list":
		for anim_name in _player.get_animation_list():
			var a := _player.get_animation(anim_name)
			print("CLIP: ", anim_name, " len=", a.length, " frames=", int(a.length * FPS))
		# Volcar las pistas de los clips de ojos para saber qué animan (hueso o blendshape).
		for anim_name in _player.get_animation_list():
			if anim_name.contains("eye"):
				var a := _player.get_animation(anim_name)
				for ti in range(a.get_track_count()):
					print("EYETRACK ", anim_name, " type=", a.track_get_type(ti), " path=", a.track_get_path(ti))
		# El Skeleton3D y sus huesos no se materializan hasta que el AnimationPlayer
		# reproduce una vez (mismo motivo por el que el probe sí los halla y un dump
		# en frio no). Reproducir wait + esperar un frame antes de volcar.
		_player.play(_find_animation("wait"))
		_player.seek(0.0, true)
		await RenderingServer.frame_post_draw
		for child in _model.find_children("*", "MeshInstance3D", true, false):
			print("MESH: ", child.name)
			var m := child as MeshInstance3D
			if m.mesh != null:
				for bs in range(m.mesh.get_blend_shape_count()):
					print("BLENDSHAPE: ", m.mesh.get_blend_shape_name(bs))
		for sk in _model.find_children("*", "Skeleton3D", true, false):
			print("SKELETON: ", sk.name, " bones=", (sk as Skeleton3D).get_bone_count())
			for b in range((sk as Skeleton3D).get_bone_count()):
				print("BONE: ", (sk as Skeleton3D).get_bone_name(b))
		print("=== DONE ===")
		get_tree().quit(0)
		return

	# Normaliza con la pose wait f0, no con la pose de reposo del FBX: en algunos
	# modelos (Morgan Berserker) el reposo es gigante y la pose real queda a media altura.
	var wait_clip := _find_animation("wait")
	_player.play(wait_clip)
	_player.pause()
	_player.seek(0.0, true)
	var head_raw := _head_position()
	var s := 15.0 / head_raw.y if head_raw != Vector3.INF and head_raw.y > 0.0001 else 1000.0
	_model.scale = Vector3.ONE * s
	print("SCALE: ", s)

	# Volcado de huesos en el contexto donde el esqueleto SI existe (el list pass
	# en frio devuelve 0 huesos). Solo en debug, para diagnosticar nombres de cara.
	if PASS == "debug" and _skeleton != null:
		print("RBONECOUNT: ", _skeleton.get_bone_count())
		for b in range(_skeleton.get_bone_count()):
			print("RBONE: ", _skeleton.get_bone_name(b))

	if PASS == "probe":
		_probe_motion()
		print("=== DONE ===")
		get_tree().quit(0)
		return

	_setup_meshes()
	_setup_camera()

	if PASS == "measure":
		await _measure()
	elif PASS == "debug":
		await _debug_snaps()
	elif PASS == "faceexp":
		await _face_experiment()
	else:
		await _save_cropped()
	print("=== DONE ===")
	get_tree().quit(0)

# Experimento de diagnostico: en el frame idle 77 barre cada hueso de expresion de
# ojos por separado, prueba reproducir el clip eye_open, y baja el alpha scissor,
# para descubrir QUE realmente muestra los ojos del modelo de verano (704710).
func _face_experiment() -> void:
	var clip := _find_animation("wait")
	var frame := 77
	var eyes := ["joint_eye_normal", "joint_eye_close", "joint_eye_re", "joint_eye_smile"]
	# 0) eye_open clip encima de la pose body (test fresco, antes de tocar huesos)
	_player.play(clip); _player.pause()
	await _pose_anchored(clip, frame, INF)
	var eo := _find_animation("eye_open")
	if _player.has_animation(eo):
		_player.play(eo); _player.seek(_player.get_animation(eo).length, true)
		_player.play(clip); _player.seek(float(frame) / FPS, true)
		await RenderingServer.frame_post_draw
		_capture().save_webp(ProjectSettings.globalize_path("res://faceexp_eyeopen.webp"), true, 0.95)
		print("FACEEXP eyeopen")
	# 1) barrido de cada expresion de ojos (resto a 0)
	var configs := {
		"normal": "joint_eye_normal", "re": "joint_eye_re",
		"smile": "joint_eye_smile", "close": "joint_eye_close",
	}
	for cfg in configs:
		_player.play(clip); _player.seek(float(frame) / FPS, true)
		for j in eyes:
			var bi := _skeleton.find_bone(j)
			if bi >= 0: _skeleton.set_bone_pose_scale(bi, Vector3.ONE * (1.0 if j == configs[cfg] else 0.0))
		await RenderingServer.frame_post_draw
		_capture().save_webp(ProjectSettings.globalize_path("res://faceexp_%s.webp" % cfg), true, 0.95)
		print("FACEEXP ", cfg)
	# 2) todas las expresiones de ojos a 1 (por si el rig las quiere todas visibles)
	_player.play(clip); _player.seek(float(frame) / FPS, true)
	for j in eyes:
		var bi := _skeleton.find_bone(j)
		if bi >= 0: _skeleton.set_bone_pose_scale(bi, Vector3.ONE)
	await RenderingServer.frame_post_draw
	_capture().save_webp(ProjectSettings.globalize_path("res://faceexp_all1.webp"), true, 0.95)
	print("FACEEXP all1")
	# 3) alpha scissor bajo + normal=1 (por si la linea del ojo cae bajo el umbral 0.4)
	for child in _model.find_children("*", "MeshInstance3D", true, false):
		var mi := child as MeshInstance3D
		if not mi.visible: continue
		for su in range(mi.get_surface_override_material_count()):
			var m := mi.get_surface_override_material(su) as StandardMaterial3D
			if m != null: m.alpha_scissor_threshold = 0.02
	_player.play(clip); _player.seek(float(frame) / FPS, true)
	for j in eyes:
		var bi := _skeleton.find_bone(j)
		if bi >= 0: _skeleton.set_bone_pose_scale(bi, Vector3.ONE * (1.0 if j == "joint_eye_normal" else 0.0))
	await RenderingServer.frame_post_draw
	_capture().save_webp(ProjectSettings.globalize_path("res://faceexp_lowalpha.webp"), true, 0.95)
	print("FACEEXP lowalpha")

# Guarda capturas sin recorte (canvas completo) del primer/medio/ultimo frame
# de cada ventana, para inspeccion visual.
func _debug_snaps() -> void:
	for anim in SELECT[_model_id].keys():
		var clip := _find_animation(_clip_for(anim))
		_player.play(clip)
		_player.pause()
		var frames := _selected_frames(anim)
		# Todos los frames de la ventana (el step de SELECT controla la densidad).
		var picks := frames
		var anchor_z := INF
		for i in picks:
			anchor_z = await _pose_anchored(clip, i, anchor_z)
			var img := _capture()
			img.save_webp(ProjectSettings.globalize_path("res://debug_%s_%03d.webp" % [anim, i]), true, 0.9)
		print("DEBUG ", anim, ": ", picks)

# Imprime cuanto se mueve el esqueleto por frame en los clips de accion,
# para elegir las ventanas SELECT sin renderizar.
func _probe_motion() -> void:
	for clip_short in ["attack_a", "attack_b", "attack_q", "attack_ex", "spell", "damage_01"]:
		var clip := _find_animation(clip_short)
		_player.play(clip)
		_player.pause()
		var total := int(_player.get_animation(clip).length * FPS)
		var prev := {}
		for i in range(total):
			_player.seek(float(i) / FPS, true)
			var cur := {}
			var motion := 0.0
			for b in range(_skeleton.get_bone_count()):
				var p: Vector3 = _skeleton.get_bone_global_pose(b).origin
				cur[b] = p
				if prev.has(b):
					motion += prev[b].distance_to(p)
			prev = cur
			print("MOTION %s %d %.3f" % [clip_short, i, motion])

func _clip_for(anim: String) -> String:
	if CLIP_OVERRIDE.has(_model_id) and CLIP_OVERRIDE[_model_id].has(anim):
		return CLIP_OVERRIDE[_model_id][anim]
	return CLIP_FOR[anim]

func _selected_frames(anim: String) -> Array:
	if FRAMES_OVERRIDE.has(_model_id) and FRAMES_OVERRIDE[_model_id].has(anim):
		return FRAMES_OVERRIDE[_model_id][anim].duplicate()
	var w: Array = SELECT[_model_id][anim]
	var frames := []
	var i: int = w[0]
	while i <= w[1]:
		frames.append(i)
		i += w[2]
	return frames

func _pose_frame(clip: String, frame_idx: int) -> void:
	_player.seek(float(frame_idx) / FPS, true)
	_apply_face_pose()

func _capture() -> Image:
	return get_viewport().get_texture().get_image()

func _measure() -> void:
	var union := Rect2i()
	var first := true
	for anim in SELECT[_model_id].keys():
		if MEASURE_SKIP.has(_model_id) and anim in MEASURE_SKIP[_model_id]:
			continue
		var clip := _find_animation(_clip_for(anim))
		_player.play(clip)
		_player.pause()
		var anchor_z := INF
		for i in _selected_frames(anim):
			anchor_z = await _pose_anchored(clip, i, anchor_z)
			var r := _capture().get_used_rect()
			if r.size.x == 0:
				continue
			union = r if first else union.merge(r)
			first = false
	print("UNION: ", union.position.x, " ", union.position.y, " ", union.size.x, " ", union.size.y)
	var fa := FileAccess.open(CROP_FILE, FileAccess.READ_WRITE if FileAccess.file_exists(CROP_FILE) else FileAccess.WRITE)
	fa.seek_end()
	fa.store_line("%d %d %d %d" % [union.position.x, union.position.y, union.size.x, union.size.y])
	fa.close()

func _merged_crop() -> Rect2i:
	var union := Rect2i()
	var first := true
	var fa := FileAccess.open(CROP_FILE, FileAccess.READ)
	while not fa.eof_reached():
		var parts := fa.get_line().split(" ", false)
		if parts.size() == 4:
			var r := Rect2i(int(parts[0]), int(parts[1]), int(parts[2]), int(parts[3]))
			union = r if first else union.merge(r)
			first = false
	fa.close()
	# Margen de seguridad, dentro del lienzo.
	union = union.grow(16).intersection(Rect2i(0, 0, 2048, 2048))
	return union

func _save_cropped() -> void:
	var crop := _merged_crop()
	var ground_row_full := (CAM_SIZE * 0.32 + CAM_SIZE * 0.5) / CAM_SIZE * 2048.0
	print("CROP: ", crop.position.x, " ", crop.position.y, " ", crop.size.x, " ", crop.size.y)
	print("GROUND_ROW_IN_CROP: ", ground_row_full - crop.position.y)
	print("CAM_CENTER_COL_IN_CROP: ", 1024.0 - crop.position.x)
	for anim in SELECT[_model_id].keys():
		var clip := _find_animation(_clip_for(anim))
		var dir: String = OUT_DIR + "/" + anim
		DirAccess.make_dir_recursive_absolute(ProjectSettings.globalize_path(dir))
		_player.play(clip)
		_player.pause()
		var anchor_z := INF
		var n := 0
		for i in _selected_frames(anim):
			anchor_z = await _pose_anchored(clip, i, anchor_z)
			var img := _capture().get_region(crop)
			img.save_webp(ProjectSettings.globalize_path("%s/%03d.webp" % [dir, n]), true, 0.9)
			n += 1
		print("  ", anim, ": ", n, " frames")

func _pose_anchored(clip: String, frame_idx: int, anchor_z: float) -> float:
	_model.position = Vector3.ZERO
	_pose_frame(clip, frame_idx)
	var head := _head_position()
	var z := head.z if head != Vector3.INF else 0.0
	if anchor_z == INF:
		anchor_z = z
	_model.position = Vector3(0, 0, anchor_z - z)
	await RenderingServer.frame_post_draw
	return anchor_z

func _setup_meshes() -> void:
	var atlas: Texture2D = load("res://" + _model_id + ".png")
	var best_body := ""
	for child in _model.find_children("*", "MeshInstance3D", true, false):
		var nm := String(child.name)
		if nm.begins_with("body") and nm > best_body:
			best_body = nm
	for child in _model.find_children("*", "MeshInstance3D", true, false):
		var mi := child as MeshInstance3D
		var nm := String(mi.name)
		var show := not nm.begins_with("body") or nm == best_body
		if HIDE_MESHES.has(_model_id):
			for pat in HIDE_MESHES[_model_id]:
				if nm.contains(pat):
					show = false
		mi.visible = show
		if not show:
			continue
		var mat := StandardMaterial3D.new()
		mat.albedo_texture = atlas
		mat.shading_mode = BaseMaterial3D.SHADING_MODE_UNSHADED
		mat.transparency = BaseMaterial3D.TRANSPARENCY_ALPHA_SCISSOR
		mat.alpha_scissor_threshold = 0.4
		mat.cull_mode = BaseMaterial3D.CULL_DISABLED
		for su in range(mi.get_surface_override_material_count()):
			mi.set_surface_override_material(su, mat)

func _setup_camera() -> void:
	var cam := Camera3D.new()
	cam.projection = Camera3D.PROJECTION_ORTHOGONAL
	cam.size = CAM_SIZE
	var head := _head_position()
	var center := Vector3(0, CAM_SIZE * 0.32, head.z if head != Vector3.INF else 0.0)
	add_child(cam)
	cam.look_at_from_position(center + Vector3(100.0, 0, 0), center, Vector3.UP)
	cam.current = true

	var light := DirectionalLight3D.new()
	light.rotation_degrees = Vector3(-30, 20, 0)
	light.light_energy = 1.2
	add_child(light)
	var env := WorldEnvironment.new()
	var e := Environment.new()
	e.ambient_light_source = Environment.AMBIENT_SOURCE_COLOR
	e.ambient_light_color = Color(1, 1, 1)
	e.ambient_light_energy = 1.0
	env.environment = e
	add_child(env)

func _head_position() -> Vector3:
	if _skeleton == null:
		_skeleton = _model.find_child("Skeleton3D", true, false)
		if _skeleton == null:
			return Vector3.INF
	var idx := _skeleton.find_bone("joint_head")
	if idx < 0:
		return Vector3.INF
	return (_skeleton.global_transform * _skeleton.get_bone_global_pose(idx)).origin

func _find_animation(clip: String) -> String:
	for anim_name in _player.get_animation_list():
		if anim_name == clip or anim_name.ends_with("|" + clip) or anim_name.ends_with("/" + clip):
			return anim_name
	# Modelos de verano/Castoria: clips con sufijo de ascension ("wait_level_3").
	# El prefijo con "_level" evita que "attack_b" agarre "attack_b02_level_3".
	for anim_name in _player.get_animation_list():
		if anim_name.begins_with(clip + "_level"):
			return anim_name
	for anim_name in _player.get_animation_list():
		if anim_name.begins_with(clip):
			return anim_name
	return clip

func _apply_face_pose() -> void:
	if _skeleton == null:
		return
	for joint in FACE_POSE.keys():
		var idx := _skeleton.find_bone(joint)
		if idx >= 0:
			_skeleton.set_bone_pose_scale(idx, Vector3.ONE * FACE_POSE[joint])
	if HIDE_BONES.has(_model_id):
		for joint in HIDE_BONES[_model_id]:
			var idx := _skeleton.find_bone(joint)
			if idx >= 0:
				_skeleton.set_bone_pose_scale(idx, Vector3.ONE * 0.0001)
