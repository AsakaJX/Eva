import json
import sys
from websocket import create_connection

ws = create_connection("ws://0.0.0.0:8001")

def createAuthKey():
    ws.send(json.dumps(
        {
            "apiName": "VTubeStudioPublicAPI",
            "apiVersion": "1.0",
            "requestID": "0",
            "messageType": "AuthenticationTokenRequest",
            "data": {
                "pluginName": "EvaPlugin",
                "pluginDeveloper": "asaka"
            }
        }
    ))

    result = json.loads(ws.recv())["data"]

    tokenFile = open("VTubeStudioAuthKey.txt", "w")
    tokenFile.write(result["authenticationToken"])
    tokenFile.close()

def tryAuth():
    tokenFile = open("VTubeStudioAuthKey.txt", "r")
    token = tokenFile.read()
    tokenFile.close()

    ws.send(json.dumps(
        {
            "apiName": "VTubeStudioPublicAPI",
            "apiVersion": "1.0",
            "requestID": "0",
            "messageType": "AuthenticationRequest",
            "data": {
                "pluginName": "EvaPlugin",
                "pluginDeveloper": "asaka",
                "authenticationToken": token
            }
        }
    ))
    result = json.loads(ws.recv())["data"]
    if "errorID" in result:
        print(f'### Auth error: {result["message"]}')
        return False
    
    print(f'### Authenticated: {result["authenticated"]} ###\n')
    return result["authenticated"]

if tryAuth() == False:
    createAuthKey()
    tryAuth()

ws.send(json.dumps(
    {
        "apiName": "VTubeStudioPublicAPI",
        "apiVersion": "1.0",
        "requestID": "SomeID",
        "messageType": "HotkeysInCurrentModelRequest"
    }
))

result = json.loads(ws.recv())
hotKeysArray = []
maxHotKeys = 0
for entity in result["data"]["availableHotkeys"]:
    print(f'{maxHotKeys + 1} | Animation name: {entity["name"]} | ID: {entity["hotkeyID"]}')
    hotKeysArray.append(entity["hotkeyID"])
    maxHotKeys = maxHotKeys + 1

print(f"\nHotKeys count: {maxHotKeys}")
print(f'Requested animation: {sys.argv[1]}')
choosenHotkey = hotKeysArray[int(sys.argv[1]) - 1]

ws.send(json.dumps(
    {
        "apiName": "VTubeStudioPublicAPI",
        "apiVersion": "1.0",
        "requestID": "0",
        "messageType": "HotkeyTriggerRequest",
        "data": {
            "hotkeyID": choosenHotkey
        }
    }
))

ws.close()