from typing import Dict
import json

class iris_container:
    def __init__(self, componentDict:Dict,
                 target:int=-1,
                 species:str=''):
        self.SepalLength = 0
        self.SepalWidth = 0
        self.PetalLength = 0
        self.PetalWidth = 0
        
        for comp in componentDict:
            if comp == 'SepalLength':
                self.SepalLength = componentDict[comp]
            elif comp == 'SepalWidth':
                self.SepalWidth = componentDict[comp]
            elif comp == 'PetalLength':
                self.PetalLength = componentDict[comp]
            elif comp == 'PetalWidth':
                self.PetalWidth = componentDict[comp]
        self.Target = target
        self.Species = species

    def toJSON(self):
        return json.dumps(self, default=lambda o:o.__dict__, 
            sort_keys=True, indent=4)

    def toDict(self):
        return self.__dict__


class xyz_container:
    def __init__(self, componentDict:Dict,
                 target:int=-1,
                 classStr:str='',
                 index:str=-2):
        self.x = 0
        self.y = 0
        self.z = 0
        
        for comp in componentDict:
            if comp == 'x':
                self.x = componentDict[comp]
            elif comp == 'y':
                self.y = componentDict[comp]
            elif comp == 'z':
                self.z = componentDict[comp]
        self.target = target
        self.ClassStr = classStr
        self.index = index

    def toJSON(self):
        return json.dumps(self, default=lambda o:o.__dict__, 
            sort_keys=True, indent=4)

    def toDict(self):
        return self.__dict__