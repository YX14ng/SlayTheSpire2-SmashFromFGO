extends AnimatedSprite2D
# Vuelve a reposo al terminar cualquier animacion de una sola ejecucion.

func _ready() -> void:
	animation_finished.connect(_on_animation_finished)
	play("idle")

func _on_animation_finished() -> void:
	if animation != &"die":
		play("idle")
