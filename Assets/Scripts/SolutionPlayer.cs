using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// custom comparer by y
public class vector3Comparer : IComparer<Vector3>
{
    //     Array.Sort((a, b) => a.x.CompareTo(b.x));
    public int Compare(Vector3 p1, Vector3 p2)
    {
        return (p1.y.CompareTo(p2.y));
    }
}

public class SolutionPlayer : MonoBehaviour
{
    public Dropdown dropdown;
    public Button pauseButton;
    public Button startButton;
    public Text status;
    public float duration = 3; // 3 sec by default

    //private string prevSoln; // stores the previous solution name
    private GameObject solution;
    //private Transform[] blocks; // stores a copy of each solution object's children
                                // re-ordered by their y in ascending order (bottom to top)
    private Vector3[] startPos;
    private Vector3[] endPos;
    private Quaternion[] startRot;
    private Quaternion[] endRot;
    private int numBlocks;
    private bool isPaused; // if animation is in progress
    private bool isPrevDone; // if the previous animation is finished
    private bool isOthersDeact; // if solutions other than the choice are deactivated
    private bool isInit; // if a solution is already initialized
    private bool solnChosen;

    void Start()
    {
        isPaused = false;
        isPrevDone = true;
        isOthersDeact = false;
        isInit = false;
        solnChosen = false;
    }

    void Update()
    {
        if (chooseSolution())
        {
            initializeSolution();
        }
    }
    
    // get the solution choice from the Dropdown
    bool chooseSolution()
    {
        string name = dropdown.GetComponent<DropdownButton>().solutionName();
        string dropdownLabel = dropdown.options[0].text;
        // check if the current option is the dropdown label
        if (!(name.Equals(dropdownLabel)))
        {
            solution = GameObject.Find(name);
            return !(solution == null);
        }
        return false;
    }

    // initialize and get pos & ori of a solution
    void initializeSolution()
    {
        // check if a real solution (not dropdown label) is selected
        // and if this solution is already initialized
        if (isPrevDone && !isInit)
        {
            // fetch the script on the PuzzleSolution object
            PuzzleAnimator animator = solution.gameObject.GetComponent<PuzzleAnimator>();

            // initialize the PuzzleAnimator
            animator.initialize();

            // get all the pos & ori info of that solution
            numBlocks = animator.getNumPieces();
            startPos = animator.getStartPos();
            endPos = animator.getEndPos();

            /*// get all children blocks
            blocks = new Transform[numBlocks];
            for (int i = 0; i < numBlocks; i++)
            {
               blocks[i] = solution.gameObject.transform.GetChild(i);
            }*/            

            startRot = animator.getStartRot();
            endRot = animator.getEndRotQuat();

            isInit = true;
        }
        
    }

    // deactivate solutions other than the selected one
    void deactivateOthers()
    {
        Debug.Log("deactivating solutions except " + solution.name);
        GameObject others;
        string othersName;
        foreach (Dropdown.OptionData opt in dropdown.options)
        {
            othersName = opt.text;
            bool isLabel = othersName.Equals(dropdown.options[0].text);
            bool isSolution = othersName.Equals(solution.name);
            if (!isLabel && !isSolution)
            {
                others = GameObject.Find(othersName);
                others.SetActive(false);
            }
        }

        isOthersDeact = true;
    }

    // reactivate other solutions
    void activateAll()
    {
        Debug.Log("activating all inactive solutions");
        GameObject others;
        string othersName;
        foreach (Dropdown.OptionData opt in dropdown.options)
        {
            othersName = opt.text;
            //Debug.Log(othersName);
            bool isLabel = othersName.Equals(dropdown.options[0].text);
            bool isSolution = othersName.Equals(solution.name);
            if (!isLabel && !isSolution)
            {
                //Debug.Log(othersName);
                others = GameObject.Find(othersName);
                Debug.Log(others);
                others.SetActive(true);
            }
        }

        isOthersDeact = false;
    }

    // put back blocks to their original positions
    // TOFIX: ***NOT working for some puzzles***
    public void restartPos()
    {
        for (int i = 0; i < numBlocks; i++)
        {
            Transform child = solution.gameObject.transform.GetChild(i);
            child.position = startPos[i];
            child.rotation = startRot[i];
        }
    }

    // update pause/resume status of the animation
    public void onPauseButtonClick()
    {
        Text pauseButtonText = pauseButton.gameObject.GetComponentInChildren<Text>();

        if (!isPaused && !(pauseButtonText.text.Equals("Resume")))
        {
            isPaused = true;
        }
        if (isPaused && pauseButtonText.text.Equals("Resume"))
        { 
            isPaused = false;
        }
    }

    // run animation on startButton click
    public void onStartButtonClick()
    {
        //if (!isOthersDeact) deactivateOthers();

        // sync PauseButton label later by calling clearCounter()
        ChangePauseButton change = pauseButton.gameObject.GetComponent<ChangePauseButton>();
        Text startButtonText = startButton.gameObject.GetComponentInChildren<Text>();

        // start animation only when previous animation is finished
        if (solution != null && isPrevDone)
        {
            // For debugging
            Debug.Log("Start animating " + solution.gameObject.name);

            isPaused = false;
            change.clearCounter();
            restartPos();

            StartCoroutine(AnimateSolution());

            //activateAll();
        }
    }

    // isn't called bc not working yet
/*    void orderBlockByY()
    {
        // Instantiate the custom comparer
        vector3Comparer myComparer = new vector3Comparer();

        Array.Sort(endPos, blocks, myComparer);
    }*/

    IEnumerator AnimateSolution()
    {
        isPrevDone = false;
        for (int i = 0; i < numBlocks; i++)
        {
            //Transform child = blocks[i];
            Transform child = solution.gameObject.transform.GetChild(i);
            // show on screen which block is being animated
            float percentage = (float)(i + 1) / numBlocks * 100;
            status.text = "Status: Animating " + child.name + 
                            ", Completeness: " + percentage.ToString("0.00") + " %";

            for (float time = 0; time < duration; time += Time.deltaTime)
            {
                while (isPaused)
                {
                    yield return null;
                }

                float u = time / duration;
                child.position = Vector3.Lerp(startPos[i], endPos[i], u);
                child.rotation = Quaternion.Slerp(startRot[i], endRot[i], u);
                yield return null;
            }

            child.position = endPos[i];
            child.rotation = endRot[i];
        }
        isPrevDone = true;
    }
}
