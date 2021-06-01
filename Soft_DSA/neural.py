from PIL import Image, ImageDraw, ImageEnhance
import os
import sys
import re
import win32pipe, win32file
#from tkinter import messagebox

import numpy as np
import keras
import tensorflow as tf
from keras.datasets import mnist
from keras.models import Sequential, Model 
from keras.layers import Dense, Dropout, Flatten, Average
from keras.layers import Input, Convolution2D, MaxPooling2D, Conv2D
from keras import backend as K
from keras.preprocessing.image import ImageDataGenerator # data augmentation
from keras.layers.normalization import BatchNormalization # batch normalisation
from keras.regularizers import l2 # L2-regularisation
from keras.callbacks import Callback, ModelCheckpoint

#параметры датасета
size = 64                         #размер стороны изображения
sign_num = 23                     #общее число подписей
train_num = 15                    #количество подписей в обучающей выборке
test_num = sign_num - train_num   #количество подписей в тестовой выборке
input_shape = (size, size, 1)     #расположение данных в датасете
num_classes = 7                   #количество пользователей
epochs = 1000                     #количество эпох
batch_size = 256                  #размер генерируемого блока данных

kernel_size = 3                   # we will use 3x3 kernels throughout
pool_size = 2                     # we will use 2x2 pooling throughout
drop_prob_1 = 0.25                # dropout after pooling with probability 0.25
drop_prob_2 = 0.5                 # dropout in the FC layer with probability 0.5
conv_depth = 32                   # use 32 kernels in both convolutional layers
hidden_size = 128                 # there will be 128 neurons in both hidden layers
l2_lambda = 0.0001                # use 0.0001 as a L2-regularisation factor
ens_models = 3                    # we will train three separate models on the data

num = 0
folder = "D:/Soft_DSA/png/"
files=os.listdir(path=folder)
for f in files:    
    if os.path.isdir(folder + f):
        num=num+1
        
num_classes = num
print(num_classes)
#print(points.shape)


inp = Input(shape=(input_shape[0], input_shape[1], input_shape[2])) # N.B. Keras expects channel dimension first
inp_norm = BatchNormalization(axis=1)(inp) # Apply BN to the input (N.B. need to rename here)


outs = [] # the list of ensemble outputs
for i in range(ens_models):
    # Conv [32] -> Conv [32] -> Pool (with dropout on the pooling layer), applying BN in between
    conv_1 = Convolution2D(conv_depth,(kernel_size, kernel_size), activation='relu', padding='same', kernel_initializer='he_uniform', kernel_regularizer=l2(l2_lambda), input_shape=input_shape)(inp_norm)
    conv_1 = BatchNormalization(axis=1)(conv_1)
    conv_2 = Convolution2D(conv_depth*2, (kernel_size, kernel_size), activation='relu', padding='same', kernel_initializer='he_uniform', kernel_regularizer=l2(l2_lambda),)(conv_1)
    conv_2 = BatchNormalization(axis=1)(conv_2)
    pool_1 = MaxPooling2D(pool_size=(pool_size, pool_size))(conv_2)
    drop_1 = Dropout(drop_prob_1)(pool_1)
    flat = Flatten()(drop_1)
    #flat = Flatten()(conv_2)
    hidden = Dense(hidden_size, kernel_initializer='he_uniform', kernel_regularizer=l2(l2_lambda), activation='relu')(flat) # Hidden ReLU layer
    #hidden = Dense(hidden_size, kernel_initializer='he_uniform', kernel_regularizer=l2(l2_lambda), activation='relu')(conv_2) # Hidden ReLU layer
    hidden = BatchNormalization(axis=1)(hidden)
    drop = Dropout(drop_prob_2)(hidden)
    outs.append(Dense(num_classes, kernel_initializer='he_uniform', kernel_regularizer=l2(l2_lambda), activation='softmax')(drop)) # Output softmax layer

out = Average()(outs)
model = Model(inp, out) 
model.compile(loss='categorical_crossentropy', # using the cross-entropy loss function
              optimizer='adam', # using the Adam optimiser
              metrics=['accuracy']) # reporting the accuracy
'''
#создаем модель нейронной сети
model = Sequential()
model.add(Conv2D(32, kernel_size=(3, 3),activation='relu',input_shape=input_shape))
model.add(Conv2D(64, (2, 2), activation='relu'))
model.add(MaxPooling2D(pool_size=(2, 2)))
model.add(Flatten())
model.add(Dense(128, activation='relu', kernel_regularizer=l2(l2_lambda)))
model.add(Dense(num_classes, activation='softmax'))
model.compile(loss='categorical_crossentropy',
              optimizer='adam',
              metrics=['accuracy'])
'''

model.load_weights('D:/Soft_DSA/sign_model_64.h5')

try:
    handle = win32file.CreateFile(
        r'\\.\pipe\neural_pipe',
        win32file.GENERIC_READ | win32file.GENERIC_WRITE,
        0,
        None,
        win32file.OPEN_EXISTING,
        0,
        None)
except pywintypes.error as e:
    if e.args[0] == 2:
        print("no pipe")
        sys.exit(-1)
    elif e.args[0] == 109:
        print("broken pipe")
        sys.exit(-1)
        
sids = np.load('D:/Soft_DSA/sids.npy')
print(sids)

while True:
    code = win32file.ReadFile(handle, 1024)
    print(code)
    if (code == (0, b'Auth')):
        print('start auth')
        picture = win32file.ReadFile(handle, 1024)
        picture = picture[1].decode("utf-8")
        image = Image.open(picture)
        image = image.convert('L')
        points = list(image.getdata())
        points = np.array(points)
        points = points.reshape((size, size))
        points = points.reshape(1, size, size, 1)
        points = points.astype('float32')
        points /= 255
        prediction = model.predict(points)
        print(prediction)
        #messagebox.showinfo(title=None, message=str(prediction))
        if (np.max(prediction, axis=1) > 0.9):
            result = np.argmax(prediction, axis=1)[0]
            result = sids[int(result)]
            print(result)
        else:
            result = 0
            i = 0
            for sid in sids:
                print(prediction[0][i])
                result = result + float(prediction[0][i]) * int(sid)
                i = i + 1
        result = int(result).to_bytes(4, 'little')
        print(result)
        win32file.WriteFile(handle, result)
            
    if (code == (0, b'Close')):
        sys.exit(0)
#prediction = model.predict(points)
#print(prediction)
#if (np.max(prediction, axis=1) > 0.95):
#    code = np.argmax(prediction, axis=1)[0]
#    os._exit(code)
#else:
#    sys.exit(-1)