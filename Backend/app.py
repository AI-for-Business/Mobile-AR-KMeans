"""usage: __main__.py
This is the python backend of the ABBA Lernfabrik.
optional arguments:
  -h, --help            show this help message and exit
"""
import argparse
import logging
import sys, os
from flask import Flask, request, render_template, Response
from pathlib import Path
import time
from werkzeug.datastructures import ImmutableMultiDict
import json

import Backend.backend.backend as back

import os
from gevent.pywsgi import WSGIServer
from gevent import monkey
import ssl

def _get_args():
    """Parses the CLI arguments
    Returns
    -------
    Namespace
        Contains all arguments
    """
    parser = argparse.ArgumentParser(
        description = '''This is the backend of the ABBA Lernfabrik.''',
        epilog = """This is a work in progress.""")    
    parser.add_argument('--run_on_server', dest='run_on_server', action='store_true',
        help='Changes the IP of the flask endpoint.')
    parser.add_argument('--ssl', dest='ssl', action='store_true',
                        help='Makes the flask endpoint accessible over https.')
    parser.set_defaults(run_on_server=False)
    parser.set_defaults(ssl=False)
    
    return parser.parse_args()

def main():
    """Entry point of the program."""
    print('--- main entered ---', flush=True)
    args = _get_args()
    
    backend = back.Backend_Unit()

    app = Flask(__name__)
    #CORS(app, resources={r'/*': {'origins': '*'}})

    print('--- backend ready ---', flush=True)

    @app.route('/get_smth', methods=['GET', 'POST'])
    def get_smth():
        if request.method =='POST':
            print('request.json: ', flush=True)
            print(request, flush=True)
            print(request.json, flush=True)

            request_content = request.json      

            if request_content['Request'] == 'dataset':
                return_arg = backend.return_json_data(request_content['Dataset'])
            elif request_content['Request'] == 'dataset_meta':
                return_arg = backend.get_dataset_features(request_content['Dataset'])
                print(return_arg, flush=True)
            elif request_content['Request'] == 'knn':
                return_arg = backend.classify_knn([request_content['PointX'], 
                                                    request_content['PointY']], 
                                                    [request_content['ComponentX'],
                                                    request_content['ComponentY']])
            elif request_content['Request'] == 'knn_3d':
                return_arg = backend.classify_knn([request_content['point_x'][0], 
                                                    request_content['point_y'][0],
                                                    request_content['point_z'][0]], 
                                                    [request_content['components_x'][0],
                                                    request_content['components_y'][0],
                                                    request_content['components_z'][0]])
            elif request_content['Request'] == 'kmeans':
                return_arg = json.dumps(backend.clustering_kmeans([request_content['ComponentX'],
                                                    request_content['ComponentY'],
                                                    request_content['ComponentZ']],
                                                    3))
            elif request_content['Request'] == 'kmeans_xyz':
                return_arg = json.dumps(backend.clustering_kmeans_xyz([request_content['ComponentX'],
                                                    request_content['ComponentY'],
                                                    request_content['ComponentZ']],
                                                    3, request_content['Dataset']))
            elif request_content['Request'] == 'visualize_xyz':
                return_arg = json.dumps(backend.visualize_xyz([request_content['ComponentX'],
                                                    request_content['ComponentY'],
                                                    request_content['ComponentZ']],
                                                    request_content['Dataset']))
            elif request_content['Request'] == 'axis':
                return_arg = backend.get_axis_scales([request_content['ComponentX'],
                                                    request_content['ComponentY'],
                                                    request_content['ComponentZ']],
                                                    request_content['Dataset'])
            elif request_content['Request'] == 'load_custom_datasets':
                return_arg = backend.load_custom_datasets()
            elif request_content['Request'] == 'legend':
                return_arg = backend.create_legend(request_content['Dataset'])
            elif request_content['Request'] == 'StartUp':
                print("StartUp confirmed", flush=True)
                return_arg = "StartUp confirmed"
            elif request_content['Request'] == 'kmeans_ex':
                return_arg = backend.kmeans_ex([request_content['ComponentX'],
                                                    request_content['ComponentY'],
                                                    request_content['ComponentZ']],
                                                    3, request_content['Dataset'],
                                                    request_content['DatapointClasses'])
            else: 
                return_arg = backend.do_smth(request_content['Request'])            
            return return_arg
        elif request.method == 'GET':
            return 'GET successful'

    host = '0.0.0.0' if args.run_on_server else '127.0.0.1'
    
    monkey.patch_all()

    if args.ssl:
        http_server = WSGIServer((host, 8091), app, keyfile='[PATH_TO_KEYFILE]', certfile='[PATH_TO_CERTFILE]')
    else:
        http_server = WSGIServer((host, 8091), app)
    http_server.serve_forever()

    #app.run(host=host, port=8091, debug=True, use_reloader=False)