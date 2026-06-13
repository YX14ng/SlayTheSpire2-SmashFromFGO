extends AnimatedSprite2D
# Vuelve a idle tras cualquier animacion de un solo disparo (salvo la muerte).

func _ready() -> void:
	animation_finished.connect(_on_animation_finished)
	play("idle")

func _on_animation_finished() -> void:
	if animation != &"die":
		play("idle")
