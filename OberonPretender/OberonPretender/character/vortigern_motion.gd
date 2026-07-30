extends Control

# Vortigern's official rig cannot be safely retargeted to Oberon's animations:
# its extra bones separate the face, coat and wings. Keep the intact official
# illustration and animate the sprite transform instead.

@onready var _sprite: AnimatedSprite2D = $Visuals/Sprite

var _active := false
var _base_position := Vector2.ZERO
var _base_scale := Vector2.ONE
var _idle_time := 0.0
var _last_animation := &""
var _motion: Tween


func _process(delta: float) -> void:
	var should_be_active := (
		_sprite.sprite_frames != null
		and _sprite.sprite_frames.resource_path.ends_with("oberon_frames_vortigern.tres")
	)
	if should_be_active != _active:
		_active = should_be_active
		_stop_motion()
		_last_animation = &""
		if _active:
			_base_position = _sprite.position
			_base_scale = _sprite.scale
			_idle_time = 0.0
		else:
			# FormVisuals already applied the next form's position and scale.
			_sprite.rotation = 0.0
			_sprite.modulate = Color.WHITE

	if not _active:
		return

	if _sprite.animation != _last_animation:
		_last_animation = _sprite.animation
		_begin_animation(_last_animation)

	if _last_animation == &"idle":
		_idle_time += delta
		var wave := sin(_idle_time * 2.0)
		_sprite.position = _base_position + Vector2(0.0, wave * 1.8)
		_sprite.scale = _base_scale * (1.0 + wave * 0.006)


func _begin_animation(animation: StringName) -> void:
	_stop_motion()
	_reset_pose()
	if animation == &"idle":
		return

	_motion = create_tween()
	_motion.set_trans(Tween.TRANS_QUAD)
	if animation == &"attack":
		_motion.tween_property(_sprite, "position", _base_position + Vector2(24.0, -3.0), 0.16).set_ease(Tween.EASE_OUT)
		_motion.tween_property(_sprite, "position", _base_position, 0.44).set_ease(Tween.EASE_IN_OUT)
	elif animation == &"cast":
		_motion.tween_property(_sprite, "position", _base_position + Vector2(0.0, -8.0), 0.30).set_ease(Tween.EASE_OUT)
		_motion.parallel().tween_property(_sprite, "scale", _base_scale * 1.035, 0.30).set_ease(Tween.EASE_OUT)
		_motion.tween_property(_sprite, "position", _base_position, 0.50).set_ease(Tween.EASE_IN_OUT)
		_motion.parallel().tween_property(_sprite, "scale", _base_scale, 0.50).set_ease(Tween.EASE_IN_OUT)
	elif animation == &"hurt" or animation == &"die":
		_motion.tween_property(_sprite, "position", _base_position + Vector2(-13.0, 1.0), 0.10).set_ease(Tween.EASE_OUT)
		_motion.parallel().tween_property(_sprite, "modulate", Color(1.0, 0.72, 0.72, 1.0), 0.10)
		_motion.tween_property(_sprite, "position", _base_position, 0.25).set_ease(Tween.EASE_OUT)
		_motion.parallel().tween_property(_sprite, "modulate", Color.WHITE, 0.25)


func _stop_motion() -> void:
	if _motion != null and _motion.is_valid():
		_motion.kill()
	_motion = null


func _reset_pose() -> void:
	_sprite.position = _base_position
	_sprite.scale = _base_scale
	_sprite.rotation = 0.0
	_sprite.modulate = Color.WHITE
