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

folder = "png/"

#параметры датасета
size = 64                         #размер стороны изображения
sign_num = 23                     #общее число подписей
train_num = 15                    #количество подписей в обучающей выборке
test_num = sign_num - train_num   #количество подписей в тестовой выборке
input_shape = (size, size, 1)     #расположение данных в датасете
num_classes = 7                   #количество пользователей
epochs = 1000                      #количество эпох
batch_size = 256                   #размер генерируемого блока данных

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
        pictures = os.listdir(path=user_folder)
        for picture in pictures:
          if picture.endswith(".png"):
            filename = user_folder + picture
            image = Image.open(filename)
            image = image.convert('L')
            points = list(image.getdata())
            points = np.array(points)
            points = points.reshape((size, size))
            signs.append(points)
        signs = np.array(signs)
        dataset.append(signs)  
        
num_classes = num
dataset = np.array(dataset)
#np.save("myset_" + str(size), dataset)
class EarlyStoppingByAccuracy(Callback):
    def __init__(self, monitor='accuracy', value=0.98, verbose=0):
        super(Callback, self).__init__()
        self.monitor = monitor
        self.value = value
        self.verbose = verbose

    def on_epoch_end(self, epoch, logs={}):
        current = logs.get(self.monitor)
        if current is None:
            warnings.warn("Early stopping requires %s available!" % self.monitor, RuntimeWarning)

        if current >= self.value:
            if self.verbose > 0:
                print("Epoch %05d: early stopping THR" % epoch)
            self.model.stop_training = True
            
#разбиваем на обучающую и тестовую выборки
x_train = []
x_test = []
y_train = []
y_test = []
for user in range(0, num_classes):
    np.random.shuffle(dataset[user])
    training, testing = dataset[user][:train_num,:], dataset[user][train_num:sign_num,:]
    x_train.append(training)
    x_test.append(testing)
    y_train.append([user]*train_num)
    y_test.append([user]*test_num)

#подготовка данных для обучения
x_train = np.array(x_train)
x_test = np.array(x_test)
y_train = np.array(y_train)
y_test = np.array(y_test)
x_train = x_train.reshape(x_train.shape[0]*x_train.shape[1], size, size, 1)
x_test = x_test.reshape(x_test.shape[0]*x_test.shape[1], size, size, 1)
y_train = y_train.reshape(y_train.shape[0]*y_train.shape[1])
y_test = y_test.reshape(y_test.shape[0]*y_test.shape[1])
y_train = keras.utils.to_categorical(y_train, num_classes)
y_test = keras.utils.to_categorical(y_test, num_classes)

x_train = x_train.astype('float32')
x_test = x_test.astype('float32')
x_train /= 255
x_test /= 255

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

#аугментация данных
datagen = ImageDataGenerator(
        width_shift_range=0.1,  # randomly shift images horizontally (fraction of total width)
        height_shift_range=0.1)  # randomly shift images vertically (fraction of total height)
datagen.fit(x_train)

valid_gen = ImageDataGenerator(
        width_shift_range=0.1,  # randomly shift images horizontally (fraction of total width)
        height_shift_range=0.1)  # randomly shift images vertically (fraction of total height)

valid_gen.fit(x_test)


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
model.add(Dropout(0.25))
model.add(Flatten())
model.add(Dense(128, activation='relu', kernel_regularizer=l2(l2_lambda)))
model.add(Dropout(0.5))
model.add(Dense(num_classes, activation='softmax'))
model.compile(loss='categorical_crossentropy',
              optimizer='adam',
              metrics=['accuracy'])
'''
callbacks = [
    EarlyStoppingByAccuracy(monitor='val_accuracy', value=0.99, verbose=1)
]

#обучаем модель
results = model.fit(datagen.flow(x_train, y_train, batch_size=batch_size),
                        #steps_per_epoch=math.ceil(x_train.shape[0]/batch_size),
                        epochs=epochs, verbose=1, 
                        validation_data=valid_gen.flow(x_test, y_test, batch_size=batch_size),
                        callbacks=callbacks)
print("Модель успешно обучена")

name = 'sign_model_' + str(size) + '.h5'
model.save_weights(name)