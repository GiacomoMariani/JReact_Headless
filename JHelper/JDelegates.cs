using UnityEngine.SceneManagement;

namespace JReact
{
    //the simple action
    public delegate void JAction();
    //a generic delegate
    public delegate void JGenericDelegate<T>(T item); 
    //an action related to activation
    public delegate void JActivationDelegate(bool isActive);
    //the event to track the loading
    public delegate void JFloatDelegate(float progress);

    //the event of the scene change
    public delegate void JSceneChange(Scene previousScene, Scene nextScene);
}
