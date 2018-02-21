import os, json

def globalKeys():
    return {k for k in globals()}

startKeys = globalKeys()
"$$$___$$$"
keys = globalKeys() - startKeys

def stripJson(source):
    if isinstance(source, dict):
        return {key: stripJson(value) 
                for key, value in ((k, source[k]) for k in source)
                if isinstance(key, str) and key != 'startKeys' and not key.startswith('__') and (value is None or isinstance(value, (dict, list, tuple, set, str, int, float)))}
    elif isinstance(source, (list, tuple, set)):
        return [stripJson(value) for value in source]
    elif source is None or isinstance(source, (str, int, float)):
        return source
    else:
        return str(source)

result = stripJson({key: globals()[key] for key in keys if key not in startKeys})
resultJson = json.dumps(result)
print(resultJson)
