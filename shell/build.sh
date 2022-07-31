godot --build-solutions -q --no-window 2> ./errors.txt

if [ -s ./errors.txt ]
then
	cat ./errors.txt
	rm ./errors.txt
	exit 1
fi
rm ./errors.txt
exit 0
