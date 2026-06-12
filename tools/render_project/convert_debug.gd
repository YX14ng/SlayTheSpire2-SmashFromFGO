extends SceneTree
# One-off: convierte los snaps debug webp a png para las hojas de contacto (GDI+ no lee webp).
func _init() -> void:
	for sub: String in ["504520", "704710", "704720"]:
		var base: String = "res://debug_out/" + sub
		if not DirAccess.dir_exists_absolute(base):
			continue
		for f in DirAccess.get_files_at(base):
			if f.ends_with(".webp"):
				var img := Image.load_from_file(ProjectSettings.globalize_path(base + "/" + f))
				img.save_png(ProjectSettings.globalize_path(base + "/" + f.replace(".webp", ".png")))
	print("CONVERTIDO")
	quit()
