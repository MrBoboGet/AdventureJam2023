using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TransitionDialog : MonoBehaviour
{
    // Start is called before the first frame update
    public Scene NextScene;
    public TextAsset Text;
    void Start()
    {
        FindObjectOfType<Dialog>().SetDoneAction(() => UnityEngine.SceneManagement.SceneManager.LoadScene(NextScene.name));
        FindObjectOfType<Dialog>().TextBoxes = new List<string>(Text.text.Split("\n"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
