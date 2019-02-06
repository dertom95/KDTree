/*MIT License

Copyright(c) 2018 Vili Volčini / viliwonka

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataStructures.ViliWonka.Tests {

    using KDTree;
    public enum QType {

        ClosestPoint,
        KNearest,
        Radius,
        Interval
    }

    public class KDTreeQueryTests : MonoBehaviour {

        public GameObject checkObject;

        public QType QueryType;

        public int K = 13;

        [Range(0f, 100f)]
        public float Radius = 0.1f;

        public bool DrawQueryNodes = true;

        public Vector3 IntervalSize = new Vector3(0.2f, 0.2f, 0.2f);

        List<KDGameObject> pointCloud;
        KDTree<KDGameObject> tree;

        KDQuery<KDGameObject> query;

        void Awake() {
            
            pointCloud = new List<KDGameObject>(600);
            for (int i = 0; i < 600; i++) {
                // create a unity-gameobject at random position...
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.name = "obj_" + i;
                sphere.transform.localScale = Vector3.one * 0.1f;
                sphere.transform.position = new Vector3(
                    (1f + Random.value * 5f),
                    (1f + Random.value * 5f),
                    (1f + Random.value * 5f)
                );
                sphere.transform.position += LorenzStep(sphere.transform.position) * 0.01f;

                // ...and wrap it an KDGameObject 
                pointCloud.Add(new KDGameObject(sphere));
            }

            query = new KDQuery<KDGameObject>();
            tree = new KDTree<KDGameObject>(pointCloud, 32);
        }

        Vector3 LorenzStep(Vector3 p) {

            float ρ = 28f;
            float σ = 10f;
            float β = 8 / 3f;

            return new Vector3(

                σ * (p.y - p.x),
                p.x * (ρ - p.z) - p.y,
                p.x * p.y - β * p.z
            );
        }

        void Update() {
            Debug.Log(checkObject.transform.position);
            for(int i = 0; i < tree.Count; i++) {

                tree.Points[i].Position += LorenzStep(tree.Points[i].Position) * Time.deltaTime * 0.01f;
                var gobj = (GameObject)tree.Points[i].UserObject;
                gobj.GetComponent<Renderer>().material.color = Color.white;
            }


            tree.Rebuild();

            if (query == null) {
                return;
            }

            

            Vector3 size = 0.2f * Vector3.one;

            var resultIndices = new List<int>();

            Color markColor = Color.red;
            markColor.a = 0.5f;
            Gizmos.color = markColor;

            switch (QueryType) {

                case QType.ClosestPoint: {

                        query.ClosestPoint(tree, checkObject.transform.position, resultIndices);
                    }
                    break;

                case QType.KNearest: {

                        query.KNearest(tree, checkObject.transform.position, K, resultIndices);
                    }
                    break;

                case QType.Radius: {

                        query.Radius(tree, checkObject.transform.position, Radius, resultIndices);

                    }
                    break;

                case QType.Interval: {

                        query.Interval(tree, checkObject.transform.position - IntervalSize / 2f, checkObject.transform.position + IntervalSize / 2f, resultIndices);

                    }
                    break;

                default:
                    break;
            }

            for (int i = 0; i < resultIndices.Count; i++) {
                var gobj = (GameObject)tree.Points[resultIndices[i]].UserObject;
                gobj.GetComponent<Renderer>().material.color = Color.red;
            }


        }





    }
}