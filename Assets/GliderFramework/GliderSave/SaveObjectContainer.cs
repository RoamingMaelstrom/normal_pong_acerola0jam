using UnityEngine;

namespace GliderSave
{
    public class SaveObjectContainer : MonoBehaviour
    {
        [SerializeField] string loadFromFolderPath = "SaveObjects";
        public FloatSaveObject[] floatSaveObjectArray;
        public IntSaveObject[] intSaveObjectArray;
        public StringSaveObject[] stringSaveObjectArray;

        private void Awake() 
        {
            floatSaveObjectArray = Resources.LoadAll<FloatSaveObject>(loadFromFolderPath);
            intSaveObjectArray = Resources.LoadAll<IntSaveObject>(loadFromFolderPath);
            stringSaveObjectArray = Resources.LoadAll<StringSaveObject>(loadFromFolderPath);

            int creationCounter = 0;

            foreach (var saveObject in floatSaveObjectArray)
            {
                creationCounter++;
                if (saveObject.SaveExists) saveObject.ForceUpdateLocalCopy();
                else saveObject.ResetValue();
            }

            if (creationCounter > 0) Debug.Log(string.Format("Loaded {0} FloatSaveObjects into Application", creationCounter));

            creationCounter = 0;

            foreach (var saveObject in intSaveObjectArray)
            {
                creationCounter++;
                if (saveObject.SaveExists) saveObject.ForceUpdateLocalCopy();
                else saveObject.ResetValue();
            }

            if (creationCounter > 0) Debug.Log(string.Format("Loaded {0} IntSaveObjects into Application", creationCounter));

            creationCounter = 0;

            foreach (var saveObject in stringSaveObjectArray)
            {
                creationCounter++;
                if (saveObject.SaveExists) saveObject.ForceUpdateLocalCopy();
                else saveObject.ResetValue();
            }

            if (creationCounter > 0) Debug.Log(string.Format("Loaded {0} StringSaveObjects into Application", creationCounter));
        }
    }
}
