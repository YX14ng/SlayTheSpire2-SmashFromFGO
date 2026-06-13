extends SceneTree
# Recorta la cabeza de un webp y la amplia (vecino-cercano) para inspeccion visual.
# Uso: MegaDot --headless --script crop_face.gd -- <in.webp> <out.png> <cx_frac> <cy_frac> <box> <zoom>
func _init() -> void:
	var a := OS.get_cmdline_user_args()
	var src: String = a[0]
	var dst: String = a[1]
	var cx: float = float(a[2])
	var cy: float = float(a[3])
	var box: int = int(a[4])
	var zoom: int = int(a[5])
	var img := Image.new()
	var err := img.load(src)
	if err != OK:
		print("LOAD FAIL ", src, " err=", err)
		quit(1)
		return
	var w := img.get_width()
	var h := img.get_height()
	var px := int(cx * w) - box / 2
	var py := int(cy * h) - box / 2
	px = clampi(px, 0, max(0, w - box))
	py = clampi(py, 0, max(0, h - box))
	var region := img.get_region(Rect2i(px, py, mini(box, w), mini(box, h)))
	region.resize(region.get_width() * zoom, region.get_height() * zoom, Image.INTERPOLATE_NEAREST)
	region.save_png(dst)
	print("OK ", dst, " src=", w, "x", h, " crop@", px, ",", py)
	quit(0)
