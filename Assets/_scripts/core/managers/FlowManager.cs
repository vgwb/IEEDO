using System;
using System.Collections;
using System.Collections.Generic;

namespace Ieedo
{
    /// <summary>
    /// Handles the flow of the application and its scenes.
    /// </summary>
    public class FlowManager
    {
        private Dictionary<SceneID, UIScene> Scenes = new Dictionary<SceneID,UIScene>();

        public void LoadScenes()
        {
        }

        private SceneID currentSceneID;
        /*
        public IEnumerator TransitionTo(SceneID sceneId)
        {
            //if (currentSceneID != SceneID.None) yield return Scenes[currentSceneID].CloseCO();
            currentSceneID = sceneId;
            //yield return Scenes[currentSceneID].OpenCO();
        }*/
    }
}
