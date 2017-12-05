using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderCameraController : MonoBehaviour {

  RenderTexture tempRT;

  // Use this for initialization
  void Start () {
    tempRT = new RenderTexture (1024, 1024, 24);
    GetComponent<Camera> ().targetTexture = tempRT;
  }

  // Update is called once per frame
  void Update () {

  }

  void OnPostRender () {
    if (GetComponent<Camera> ().targetTexture != null) {

      int sqr = 1024;

      Debug.Log (Application.persistentDataPath + "/p.png");

      Texture2D virtualPhoto = new Texture2D (sqr, sqr, TextureFormat.RGBA32, false);
      virtualPhoto.ReadPixels (new Rect (0, 0, sqr, sqr), 0, 0);

      byte[] bytes;
      bytes = virtualPhoto.EncodeToPNG ();

      System.IO.File.WriteAllBytes (Application.persistentDataPath + "/p.png", bytes);

      GetComponent<Camera> ().targetTexture = null;
    }
  }

}