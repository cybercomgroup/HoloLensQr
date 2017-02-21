import subprocess, os, json, datetime, wmi, urllib2, psutil
from bottle import run, post, request, response, get, route, static_file

def id_exists(data, id):
	for k, v in data.items():
		if k == "id" and v == id:
			return True
		elif isinstance(v, dict):
			if id_exists(v, id):
				return True
		elif isinstance(v, list):
			for w in v:
				if id_exists(w, id):
					return True
			
def change_value(data, id, value, field='value'):
	for k, v in data.items():
		if k == "id" and v == id:
			data[field] = value
			return True
		elif isinstance(v, dict):
			if change_value(v, id, value, field):
				return True
		elif isinstance(v, list):
			for w in v:
				if change_value(w, id, value, field):
					return True

def fixfile(file, id, value, field='value'):
	with open('json/'+file+'.json') as input_file:    
		data = json.load(input_file)
		if change_value(data, id, value, field):
			# TODO: save json to file
			with open('json/'+file+'.json', 'w') as output_file:
				output_file.writelines( json.dumps(data, indent=4) )
			return "OK"
	return "NOK"

def chat(file,id, value):
	with open('json/'+file+'.json') as input_file:
		data = json.load(input_file)
		if id=="input":
			item = {}
			time = '{:%H:%M:%S}'.format(datetime.datetime.now())
			extra = ""
			n = 1
			while id_exists(data,time+extra):
				extra = " (%d)" % (n)
				n = n+1
			item["title"] = time
			item["id"] = time + extra
			item["text"] = value
			if len(data["list"]) > 9:
				#trim list
				new_list = []
				for i in range(-9,0):
					print i
					new_list.append(data["list"][i])
				data["list"] = new_list
			data["list"].append(item)
			with open(file+'.json', 'w') as output_file:
				output_file.writelines( json.dumps(data, indent=4) )
			return "Added message"
		elif id == "clear":
			data["list"] = []
			with open('json/'+file+'.json', 'w') as output_file:
				output_file.writelines( json.dumps(data, indent=4) )
			return "Cleared"
		else:
			return "unknown command"
	return "Error"

def blink(file, id, value):
	return urllib2.urlopen("http://127.0.0.1:5000/"+id+"/"+value).read()
	
def sensor(file, id, value):
	return "not implemented"

def update_sensor(file):
	fixfile(file,'time','{:%H:%M:%S}'.format(datetime.datetime.now()), 'text')
	fixfile(file,'cpu',str(psutil.cpu_percent(interval=None)), 'text')
	for disk in wmi.WMI().Win32_LogicalDisk (DriveType=3):
		fixfile(file,'hard_drive',disk.Caption+ "%0.2f%% free" % (100.0 * long (disk.FreeSpace) / long (disk.Size)), 'text')
	
@route('/menu/<path>',method = 'GET')
def process(path):
	if path == "sensor":
		update_sensor('sensor')
	if os.path.isfile('json/'+path+'.json'):
		return static_file('json/'+path+'.json',os.getcwd())
	return static_file('json/default.json',os.getcwd())

@route('/image/<path>',method = 'GET')
def process(path):
	if os.path.isfile('image/'+path):
		return static_file('image/'+path,os.getcwd())
	return static_file('image/default.jpg',os.getcwd())

@route('/<file>/<id>/<value>',method = 'GET')
def process(file,id,value):
	if file == "chat":
		return chat(file,id,value)
	elif file == "blink":
		return blink(file,id,value)
	elif file == "test":
		return fixfile(file,id,value)
	elif file == "sensor":
		return sensor(file,id,value)
	else:
		return "Unknown command"

@route('/',method = 'GET')
def process():
	return static_file('json/default.json',os.getcwd())

#psutil.cpu_percent(interval=None)
run(host='0.0.0.0', port=80, debug=True)

