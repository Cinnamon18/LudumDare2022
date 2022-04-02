extends Node


# Declare member variables here. Examples:
# var a = 2
# var b = "text"
var speed = Vector2(300, 300)

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.

func _process(delta):
	var input = Vector2(0, 0)
	if Input.is_action_pressed("ui_right"):
		input += Vector2.RIGHT
	
	if Input.is_action_pressed("ui_left"):
		input += Vector2.LEFT
		
	if Input.is_action_pressed("ui_up"):
		input += Vector2.UP
	
	if Input.is_action_pressed("ui_down"):
		input += Vector2.DOWN

	self.position += input * delta * speed
