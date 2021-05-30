import numpy as np
import keras
import tensorflow as tf
import math
from keras.datasets import mnist
from keras.models import Sequential, Model 
from keras.layers import Dense, Dropout, Flatten, Average
from keras.layers import Input, Convolution2D, MaxPooling2D, Conv2D
from keras import backend as K
from keras.preprocessing.image import ImageDataGenerator # data augmentation
from keras.layers.normalization import BatchNormalization # batch normalisation
from keras.regularizers import l2 # L2-regularisation
from keras.callbacks import Callback, ModelCheckpoint

from PIL import Image, ImageDraw, ImageEnhance
import os
import re

folder = "./png/"

#параметры датасета
size = 64                         #размер стороны изображения
sign_num = 23                     #общее число подписей
train_num = 14                    #количество подписей в обучающей выборке
test_num = sign_num - train_num   #количество подписей в тестовой выборке
input_shape = (size, size, 1)     #расположение данных в датасете
num_classes = 7                   #количество пользователей
epochs = 1000                      #количество эпох
batch_size = 64                   #размер генерируемого блока данных

kernel_size = 3                   # we will use 3x3 kernels throughout
pool_size = 2                     # we will use 2x2 pooling throughout
drop_prob_1 = 0.25                # dropout after pooling with probability 0.25
drop_prob_2 = 0.5                 # dropout in the FC layer with probability 0.5
conv_depth = 32                   # use 32 kernels in both convolutional layers
hidden_size = 128                 # there will be 128 neurons in both hidden layers
l2_lambda = 0.0001                # use 0.0001 as a L2-regularisation factor
ens_models = 3                    # we will train three separate models on the data

def itos(num):
    if num < 10:
        return "00" + str(num)
    elif num < 100:
        return "0" + str(num)
    else:
        return str(num)

dataset = []    
num = 0
files=os.listdir(path=folder)
for f in files:    
    if os.path.isdir(folder + f):
        num=num+1
        signs = []
        user_folder = folder + f + "/"
        print(user_folder)
        pictures = os.listdir(path=user_folder)
        for picture in pictures:
          if picture.endswith(".png"):
            filename = "0.png"
            print(filename)
            image = Image.open(filename)
            #image = image.convert('L')
            points = list(image.getdata())
            print(points)
            points = np.array(points)
            points = points.reshape((size, size))
            signs.append(points)
        signs = np.array(signs)
        dataset.append(signs)  
        
num_classes = num
dataset = np.array(dataset)