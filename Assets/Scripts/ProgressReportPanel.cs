using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressReportPanel : MonoBehaviour
{
    [SerializeField] private LessonProgressDataHandler DataHandler = null;

    [SerializeField] private Transform ContentRoot = null;
    [SerializeField] private StepToggleEntry Prefab = null;

    public void CreateReport()
    {
        for (int i = ContentRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(ContentRoot.GetChild(i).gameObject);
        }

        foreach (KeyValuePair<StepData, StepStates> kvPair in DataHandler.EndedSteps)
        {
            StepToggleEntry entry = Instantiate(Prefab, ContentRoot);
            entry.transform.localPosition = Vector3.zero;
            entry.transform.localRotation = Quaternion.identity;
            entry.transform.localScale = Vector3.one;

            entry.StepName.text = kvPair.Key.StepName;
            entry.CompletionLabel.text = kvPair.Value.ToString();
        }
    }
}
