errors="0"

if [ "$1" != "--no-build" ]
then
	./shell/build.sh
	errors=$(echo $?)
fi

if [ "$errors" = "0" ]
then
	godot scenes/Tests.tscn --no-window --terminal-colored
fi
