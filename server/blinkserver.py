from Bstick import Bstick
from flask import Flask, jsonify, request
import psutil
import threading
import time
from random import randint

app = Flask(__name__)

spazing_out_flag = False
run_loop = True

bstick = Bstick.Stick(r_led_count=32, max_rgb_value=100)
if not bstick.connect():
    print ("No blinkstick found")
    exit()
else:
    bstick.clear()


def cpu_usage():
    print ('Returning cpu usage')
    return str(psutil.cpu_percent(interval=1))


def connections():
    p = psutil.Process()
    count = str(len(p.connections(kind='all')))
    return count

def hex_to_rgb(value):
    """Return (red, green, blue) for the color given as #rrggbb."""
    value = value.lstrip('#')
    lv = len(value)
    return tuple(int(value[i:i + lv // 3], 16) for i in range(0, lv, lv // 3))

@app.route('/changecolor', methods=['POST'])
def change_color_all():
    global spazing_out_flag
    spazing_out_flag = False
    payload = request.get_json()
    return bstick.change_led_color(payload['rgb'])

@app.route('/randomcolor', methods=['POST'])
def random_color_all():
    global spazing_out_flag
    spazing_out_flag = False
    return bstick.change_led_random()


@app.route('/getcpuusage', methods=['POST'])
def get_cpu_usage():
    return cpu_usage()


@app.route('/getconnections', methods=['POST'])
def get_connections():
    return connections()

@app.route('/randomblink', methods=['POST'])
def spazing_out():
    global spazing_out_flag
    spazing_out_flag = True
    return 'ok'

@app.route('/<what>/<value>', methods=['GET'])
def what_value(what, value):
#Change value to %23 -> #
	if what == "randomblink":
		return spazing_out()
	elif what == "getconnections":
		return get_connections()
	elif what == "getcpuusage":
		return get_cpu_usage()
	elif what == "randomcolor":
		return random_color_all()
	elif what == "setrandom":
		global spazing_out_flag
		spazing_out_flag = False
		return bstick.change_led_color([randint(0,255),randint(0,255),randint(0,255)])
	elif what == "changecolor":
		global spazing_out_flag
		spazing_out_flag = False
		return bstick.change_led_color(hex_to_rgb(value))
	else:
		return "What?"

@app.route('/<what>', methods=['GET'])
def what(what):
    return what_value(what, "")

def run_spaz_loop():
    global spazing_out_flag
    global run_loop
    while run_loop:
        if spazing_out_flag == True:
            bstick.spaz_out()
        time.sleep(0.01)


def stop_loop():
    global run_loop
    run_loop = False

if __name__ == '__main__':
    t = threading.Thread(target=run_spaz_loop)
    t.start()
    app.run(host='0.0.0.0', threaded=True)
    stop_loop()
    t.join()
