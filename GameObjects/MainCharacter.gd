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
	if Input.is_action_pressed("ui_right"):
		pass
	
	if Input.is_action_pressed("ui_left"):
		pass
		
	if Input.is_action_pressed("ui_up"):
		pass
	
	if Input.is_action_pressed("ui_down"):
		pass
