import numpy as np
import os
from random import randint

folder = "./png/"

sids = []    
files=os.listdir(path=folder)
for f in files:    
    if os.path.isdir(folder + f):
        number = randint(1, 2147483647)
        print(number)
        sids.append(number)
        
sids = np.array(sids)
np.save('sids.npy', sids)