using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class ButtonHandler : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IDeselectHandler
{
    //, ISelectHandler
    public Image arrow;
    public GameObject rewiredPrefab;

    private AudioManagerTitle audio;
    private GameObject canvas;
    private GameObject input;
    private GameObject events;

    private void Awake()
    {
        input  = GameObject.Find("Rewired Input Manager");
        events = GameObject.Find("EventSystem");
        canvas = GameObject.Find("Canvas");
        audio  = GameObject.FindObjectOfType<AudioManagerTitle>();
    }

    private void Update()
    {
        
    }
    public void unloadActiveScene()
    {
        audio.Play(audio.UISelection);

        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));

        UnityEngine.Object[] objList = Resources.FindObjectsOfTypeAll(typeof(GameObject));

        foreach (GameObject obj in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            // if (obj.name.Contains("Rewired"))
            //     obj.SetActive(true);
            if (obj.name.Contains("Canvas"))
                obj.SetActive(true);
            else if (obj.name.Contains("EventSystem"))   
                obj.SetActive(true); 
        }

        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject);

        // if (!GameObject.Find("Rewired Input Manager"))
        // {
        //     var newPrefab = Instantiate(rewiredPrefab);
        //     newPrefab.name = "Rewired Input Manager";
        // }

    }

    public void loadSceneByName(string sceneName)
    {
        audio.Play(audio.UISelection);

        // if we need to load second control scene, unload the current one first
        // hoepfully this will fix rewired issue
        bool loadSndCtrl = (sceneName == "ControlsDesc2");

        Debug.Log("load 2nd ctrl: " + loadSndCtrl);
        if (loadSndCtrl)
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));

        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            Destroy(input);
            input = null;

            events.SetActive(false);
            canvas.SetActive(false);
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }   
    }

    public void loadGame()
    {
        audio.Play(audio.UISelection);
        SceneManager.LoadScene("Scenes/FinalScene");
    }

    IEnumerator setNextActiveScene(string sceneName)
    {
        
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == sceneName);
        
        // arrowList[SceneManager.sceneCount - 1] = FindArrow(sceneName);
        // if (arrowList[SceneManager.sceneCount - 1] == null)
        //     Debug.Log("something is wrong when load: arrow cant find");

    }

    public void quitGame()
    {
        audio.Play(audio.UISelection);
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }


    public void exitSelect()
    {
        // arrow.enabled = false;
        // Debug.Log("exit selection");
    }

    public void enterSelect()
    {
        // arrow.transform.position = this.transform.position;
        // arrow.enabled = true;
        // Debug.Log("entered selection");
    }


    public void OnSelect(BaseEventData eventData)
    {
        if (!GameObject.Find("Rewired Input Manager"))
        {
            var newPrefab = Instantiate(rewiredPrefab);
            newPrefab.name = "Rewired Input Manager";
        }
        arrow.enabled = true;
        arrow.transform.position = this.transform.position;
        audio.Play(audio.UIToggle);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {        
        if (!EventSystem.current.alreadySelecting)
        {
            arrow.transform.position = this.transform.position;
            arrow.enabled = true;

            audio.Play(audio.UIToggle);
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }   
    }
  
    public void OnDeselect(BaseEventData eventData)
    {
        arrow.enabled = false;
        this.GetComponent<Selectable>().OnPointerExit(null);
    }
}
