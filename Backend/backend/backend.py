from asyncio import DatagramTransport
from imp import load_dynamic
import logging
from msilib.schema import Component
import numpy as np
import pandas as pd
from sklearn import datasets
from sklearn.neighbors import KNeighborsClassifier
from sklearn.cluster import KMeans
from typing import List
import json
from math import ceil, floor, log
import os
import pathlib

import Backend.container.containers as containers

class Backend_Unit:    
    """ 
    """
    def __init__(self):
        logging.info("Backend initialized")
        
    
    def do_smth(self, smth:str='smth'):
        """Does nothing, just for testing.
        Params
        ------
        string:smth
        Returns
        -------
        smth      
        """           
        
        return smth + '_else'

    def clustering_kmeans(self, components:List[str], cluster_centers:int):
        iris = self.load_sklearn_to_pd('iris')
        centroids = None
        data_list = []
        for i in range(9):
            if i == 0:
                kmeans = KMeans(n_clusters=cluster_centers, init='random', n_init=1, random_state=95, max_iter=i+1)
            else:
                kmeans = KMeans(n_clusters=cluster_centers, init=centroids, n_init=1, random_state=95, max_iter=i+1)
            kmeans.fit(iris[components])
            if (centroids == kmeans.cluster_centers_).all():
                break
            centroids=kmeans.cluster_centers_
            iris['target'] = kmeans.predict(iris[components])
            print(iris['target'], flush=True)
            iris_json = iris.to_json(orient='records')
            iris_json_list = json.loads(iris_json)
            for centroid in centroids:
                iris_json_list.append(containers.iris_container({components[0]:centroid[0], components[1]:centroid[1], components[2]:centroid[2]}, species='Centroid').toDict())
            data_list.append(iris_json_list)
        return data_list

    def dataset_xyz(self, components:List[str], dataset:str):
        if 'custom_' in dataset:
            try:
                df = self.load_dataset(dataset.replace('custom_', ''))
            except:
                print('No available dataset provided!')
        else:
            try:
                df = self.load_sklearn_to_pd(dataset)
            except:
                print('No available dataset provided!')
        df = df.astype({'target':'int'})
        df.reset_index(inplace=True)
        df.rename(columns={'level_0': 'index', components[0]: 'x', components[1]: 'y', components[2]: 'z'}, inplace=True)

        return df

    def visualize_xyz(self, components:List[str], dataset:str):
        return self.dataset_xyz(components, dataset).to_json(orient='records')

    def clustering_kmeans_xyz(self, components:List[str], cluster_centers:int, dataset:str):
        df = self.dataset_xyz(components, dataset)

        centroids = None
        data_list = []
        for i in range(9):
            if i == 0:
                kmeans = KMeans(n_clusters=cluster_centers, init='random', n_init=1, random_state=95, max_iter=i+1)
            else:
                kmeans = KMeans(n_clusters=cluster_centers, init=centroids, n_init=1, random_state=95, max_iter=i+1)
            kmeans.fit(df[['x', 'y', 'z']])
            if (centroids == kmeans.cluster_centers_).all():
                break
            centroids=kmeans.cluster_centers_
            df['target'] = kmeans.predict(df[['x', 'y', 'z']])
            df_json = df.to_json(orient='records')
            df_json_list = json.loads(df_json)
            for centroid in centroids:
                df_json_list.append(containers.xyz_container({'x': centroid[0], 'y': centroid[1], 'z': centroid[2]}, classStr='Centroid', target=-1).toDict())
            data_list.append(df_json_list)
        return data_list

    def kmeans_ex(self, components:List[str], cluster_centers:int, dataset:str, datapoint_classes):
        df = self.dataset_xyz(components, dataset)

        centroids = None
        if not datapoint_classes:
            kmeans = KMeans(n_clusters=cluster_centers, init='random', n_init=1, max_iter=1)
            kmeans.fit(df[['x', 'y', 'z']])
            # centroids = kmeans.cluster_centers_
            centroids = np.random.random_sample((3,3)) * 4

            df_json = df.to_json(orient='records')
            df_json_list = json.loads(df_json)
            i = -1
            for centroid in centroids:
                df_json_list.append(containers.xyz_container({'x': centroid[0], 'y': centroid[1], 'z': centroid[2]}, classStr='Centroid', target=-1, index=i).toDict())
                i -= 1

            return df_json_list
        else:
            for key in datapoint_classes:
                if int(key) >= 0:
                    df.iat[int(key), 4] = int(datapoint_classes[key])
            centroid_list = []
            centroid_list.append(containers.xyz_container({'x': df[df['target'] == 0].mean()['x'],
                                                          'y': df[df['target'] == 0].mean()['y'],
                                                          'z': df[df['target'] == 0].mean()['z']},
                                                          classStr='Centroid', target=-1, index=-1).toDict())
            centroid_list.append(containers.xyz_container({'x': df[df['target'] == 1].mean()['x'],
                                                          'y': df[df['target'] == 1].mean()['y'],
                                                          'z': df[df['target'] == 1].mean()['z']},
                                                           classStr='Centroid', target=-1, index=-2).toDict())
            centroid_list.append(containers.xyz_container({'x': df[df['target'] == 2].mean()['x'],
                                                          'y': df[df['target'] == 2].mean()['y'],
                                                          'z': df[df['target'] == 2].mean()['z']},
                                                           classStr='Centroid', target=-1, index=-3).toDict())
            return centroid_list
        

    def get_axis_steps(self, axis_min, axis_max, max_fact):
        number_axis_steps = round(axis_max / max_fact - axis_min / max_fact)
        step_size = max_fact
        while number_axis_steps > 10:
            if number_axis_steps % 2 != 0:
                number_axis_steps += 1
                axis_max += step_size
            step_size *= 2
            number_axis_steps /= 2
        axis_steps = []
        for step in range(0, int(number_axis_steps)):
            axis_steps.append(axis_min + (step + 1) * step_size)
        return axis_steps, axis_max

    def axis_min_max(self, mini, maxi):
        if mini == 0:
            exp = 0
        else:
            exp = ceil(log(mini, 10)) - 1
        fact = 10 ** exp
        axis_min = round(floor(mini / fact) * fact, abs(exp))
        axis_max = round(ceil(maxi / fact) * fact, abs(exp))
        if axis_min == 0:
            ceil_min = 0
        else:
            ceil_min = ceil(log(axis_min, 10))
        max_fact = 10 ** (ceil(log(axis_max, 10)) - 1 - (ceil(log(axis_max, 10)) - ceil_min))
        axis_steps, axis_max = self.get_axis_steps(axis_min, axis_max, max_fact)
        return [[axis_min], [axis_max], axis_steps]

    def get_axis_scales(self, components:List[str], dataset:str):
        df = self.dataset_loader(dataset)
        axis_dict = {}
        if components[0] != None:
            x_axis = self.axis_min_max(df[components[0]].min(), df[components[0]].max())
            axis_dict['min'] = x_axis[0]
            axis_dict['max'] = x_axis[1]
            axis_dict['steps'] = x_axis[2]
        if components[1] != None:
            y_axis = self.axis_min_max(df[components[1]].min(), df[components[1]].max())
            axis_dict['min'] = y_axis[0]
            axis_dict['max'] = y_axis[1]
            axis_dict['steps'] = y_axis[2]
        if components[2] != None:
            z_axis = self.axis_min_max(df[components[2]].min(), df[components[2]].max())
            axis_dict['min'] = z_axis[0]
            axis_dict['max'] = z_axis[1]
            axis_dict['steps'] = z_axis[2]
        return axis_dict

    def create_legend(self, dataset_name):
        if 'custom_' in dataset_name:
            return self.load_custom_dataset_features(dataset_name.replace('custom_', ''))['legend']
        else:
            dataset = self.load_sklearn_dataset(dataset_name)
            target_dict = {}
            for i in range(0, len(dataset['target_names'])):
                target_dict[i] = dataset['target_names'][i]
            return target_dict

    def classify_knn(self, points:List[float], components:List[str]):
        df = self.load_dataset('iris')
        X = df[components]
        y = df[['target']]

        neigh = KNeighborsClassifier(n_neighbors=3)

        neigh.fit(X, y)
        #points = [i.replace(',', '.') for i in points]
        return str(int(neigh.predict([[float(i) for i in points]])[0]))

    def return_json_data(self, dataset_name:str):
        df = self.load_dataset(dataset_name)
        return df.to_json(orient='records')

    def load_sklearn_dataset(self, dataset_name:str):
        if dataset_name == 'iris':
            return datasets.load_iris()
        elif dataset_name == 'wine':
            return datasets.load_wine()

    def get_dataset_features(self, dataset_name:str):
        if 'custom_' in dataset_name:
            return self.load_custom_dataset_features(dataset_name.replace('custom_', ''))['clustering']['feature_names']
        else:
            dataset = self.load_sklearn_dataset(dataset_name)
            return dataset['feature_names']

    def dataset_loader(self, dataset):
        if 'custom_' in dataset:
            try:
                df = self.load_dataset(dataset.replace('custom_', ''))
            except:
                print('No available dataset provided!')
        else:
            try:
                df = self.load_sklearn_to_pd(dataset)
            except:
                print('No available dataset provided!')
        return df

    def load_custom_dataset_features(self, dataset_name:str):
        with open('datasets/' + dataset_name + '/' + dataset_name + '.json') as json_file:
            dataset_features = json.load(json_file)

            return dataset_features

    def load_sklearn_to_pd(self, dataset_name):
        dataset = self.load_sklearn_dataset(dataset_name)

        return pd.DataFrame(data=np.c_[dataset['data'], dataset['target']],
                            columns=dataset['feature_names'] + ['target'])

    def load_dataset(self, dataset_name:str):
        """Loads and returns a pandas dataset from csv file.
        Params
        ------
        string:dataset_name
        The name of the dataset to load
        Returns
        -------
        loaded_dataset
        The pandas dataset
        """  

        dataset_string = 'datasets/' + dataset_name + '/' + dataset_name + '.csv'
        loaded_dataset = pd.read_csv(dataset_string)
        loaded_dataset['target'] = pd.to_numeric(loaded_dataset['target'], downcast='integer')

        return loaded_dataset

    def load_custom_datasets(self):
        datasets_dir = os.getcwd() + '\datasets'
        #print(datasets_dir)
        dataset_list = []
        for file in os.listdir(datasets_dir):
            d = os.path.join(datasets_dir, file)
            if os.path.isdir(d):
                dataset_name = d.split('\\')[-1]
                print(dataset_name)
                dataset_list.append(dataset_name)
        return dataset_list

    iris_dict = {'SepalLength': 'sepal length (cm)',
                'SepalWidth': 'sepal width (cm)',
                'PetalLength': 'petal length (cm)',
                'PetalWidth': 'petal width (cm)'}

    iris_dict_inv = {v: k for k, v in iris_dict.items()}