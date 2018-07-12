using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DatabaseControl;

public class DCP_CD_MainControl : MonoBehaviour {

    public Text loggedInAs; // << The UI Text which shows 'Logged in as: DisplayName'
    public Text refreshText; // << The UI Text which shows 'Refreshing in ... Time' or 'Refreshing ...'

    //The following are used for fading the UI in and out
    public DCP_UIMoveGroup loadingGroup;
    public DCP_UIDelay loadingGroupIn;
    public DCP_UIMoveGroup nameGroup;
    public DCP_UIDelay nameGroupIn;
    public DCP_UIMoveGroupList allChatUI; // << This one is added to as comments are created so the comments fade in and out with the rest of the UI. It is a based on lists rather than built-in arrays so it can be added to at runtime
    public DCP_UIDelay chatUIIn;

    //The input field for the submitted display name
    public InputField chosenName;

    //The input field for the text to be posted as a comment
    public InputField textToPost;

    //The Template Comment gameObject. This is duplicated to add more comments
    public GameObject templateComment;

    //The parent of all of the comments so the layout is correct in the scroll view
    public Transform commentsParent;

    //The gameObjects to enable/disable to show 'Sending' or the UI to post a comment
    public GameObject postCommentStuff;
    public GameObject sendingCommentStuff;

    //The name which the player has joined the chat with. It is used to run the command sequences to post comments made by the player
    string displayName = "Bob";

    //These are copies of lists which the allChatUI variable contains at the very start of the game. They are used to reset the allChatUI lists when required.
    List<DCP_UIMove1DImage> groupsImages;
    List<DCP_UIMove1DText> groupsTexts;

    //The List of created comment GameObjects
    List<GameObject> comments = new List<GameObject>();

    //These are used to make sure the demo scene has been setup correctly and to store the databse name which is used to run Command Sequences
    string databaseName = "";
    bool canRunSequences = false;

    int refreshNumber = 0; // << Every refresh is given its own number. This means it can be canceled when the overall refreshNumber has changed. This is used when the player logs out or posts a comment (which forces the next refresh to happen early)

    bool isLoggedIn = false; // << Whether the player has logged in or not
    bool isLoadingCommentsFirstTime = false; // << This is used to avoid comment UI fading in before it should when fading in all of the chat ui at the same time

    void Awake ()
    {
        //Copies the lists on the allChatUI script. These are used to reset the comments
        groupsImages = allChatUI.movesImages;
        groupsTexts = allChatUI.movesTexts;

        //Gets the databaseName as it was setup through the editor
        GameObject linkObj = GameObject.Find("Link");
        if (linkObj == null)
        {
            Debug.LogError("DCP Error: Cannot find the link object in the scene so scripts running Command Sequences don't know the database name");
        }
        else
        {
            DCP_Demos_LinkDatabaseName linkScript = linkObj.gameObject.GetComponent<DCP_Demos_LinkDatabaseName>() as DCP_Demos_LinkDatabaseName;
            if (linkScript == null)
            {
                Debug.LogError("DCP Error: Cannot find the link script on link object so scripts running Command Sequences don't know the database name");
            }
            else
            {
                if (linkScript.databaseName == "")
                {
                    Debug.LogError("DCP Error: This demo scene has not been setup. Please setup the demo scene in the Setup window before use. Widnow>Database Control Pro>Setup Window");
                }
                else
                {
                    databaseName = linkScript.databaseName;
                    canRunSequences = true;
                }
            }
        }
    }

    //This is called when the player logs in to clear any comments which were created if they were logged in before
    void DeleteAllComments ()
    {
        //Removes the created comments from the fading in and out UI
        allChatUI.movesImages = groupsImages;
        allChatUI.movesTexts = groupsTexts;

        //Destroys the comments
        foreach (GameObject c in comments)
        {
            Destroy(c);
        }

        //Makes sure the list of comments is empty
        comments = new List<GameObject>();
    }

    //This is called when the 'Join Chat' button has been pressed to submit a display name
    public void JoinChatButtonPressed ()
    {
        if (canRunSequences == true) // << Makes sure the demo scene has been setup correctly
        {
            if (string.IsNullOrEmpty(chosenName.text) == false) // << Makes sure the player has entered a name
            {
                loggedInAs.text = "Logged in as: " + chosenName.text; // << This will display the player's name later when it becomes visible

                DeleteAllComments(); //Removes all of the comments created if the player has logged in before

                //Makes sure the 'Sending...' text is not showing and the player can post comments later when it fades in
                postCommentStuff.gameObject.SetActive(true);
                sendingCommentStuff.gameObject.SetActive(false);

                //These two foreach loops set all of the alphas to 0 of the children of the two above object, as if the postCommentStuff was disabled and is now being enabled it might appear and start fading out as its MoveBack method was called when it was disabled
                //Basically, it just avoids a bug
                foreach (DCP_UIMove1DImage i in groupsImages)
                {
                    i.StopMoving();
                    Color col = i.applyToImage.image.color;
                    col.a = 0;
                    i.applyToImage.image.color = col;
                }
                foreach (DCP_UIMove1DText i in groupsTexts)
                {
                    i.StopMoving();
                    Color col = i.applyToImage.text.color;
                    col.a = 0;
                    i.applyToImage.text.color = col;
                }

                isLoadingCommentsFirstTime = true; // << This makes sure the comments don't fade in before the rest of the UI

                //Make the naming ui fade out and the loading ui fade in
                nameGroup.MoveBack();
                loadingGroupIn.StartDelay();

                displayName = chosenName.text; // << Sets the display name which will be sent with the command sequence submitting a post
                isLoggedIn = true; // << If this was set to false the refreshes would not work as it is used to stop refreshes when the player logs out

                //Run the Coroutine which runs the Command Sequence to get the Chat data
                StartCoroutine(GetChatInfo());
            }
        }
    }

    //This is called when the 'Post' Button is pressed
    public void PostCommentButtonPressed ()
    {
        if (canRunSequences == true) // << Makes sure the demo scene has been setup correctly
        {
            if (textToPost.text != "") // << Make sure the player has entered a comment
            {
                //Show 'Sending...' text and disable the post comment UI
                sendingCommentStuff.gameObject.SetActive(true);
                postCommentStuff.gameObject.SetActive(false);

                //Run the coroutine which runs the Command Sequence to post the player's comment
                StartCoroutine(SendComment(displayName, textToPost.text));

                textToPost.text = ""; // << Clear the comment input field
            }
        }
    }

    //This is called when the 'Logout' Button is pressed
    public void LogoutButtonPressed()
    {
        isLoggedIn = false; // << this prevents a currently running refresh from doing anything to the UI

        refreshNumber++; // << cancels current refresh countdown

        //Fades the Chat UI out and the choose display name UI in
        allChatUI.MoveBack();
        nameGroupIn.StartDelay();
    }

    //This is called when chat data is recieved or after a refresh to add a comment
    void AddComment (string name, string text)
    {
        //Duplicates the comment template and adds the new gameObject to the list of comments
        GameObject newComment = Instantiate(templateComment) as GameObject;
        comments.Add(newComment);

        //Set the parent of the new comment so it appears in the scroll view. The positioning of the comment is done automatically by Unity UI Layout Groups
        newComment.transform.parent = commentsParent;

        //Get the script on the comment which should have links to the important parts of the comment which we need
        DCP_CD_Comment commentScript = newComment.gameObject.GetComponent<DCP_CD_Comment>() as DCP_CD_Comment;
        if (commentScript != null)
        {
            commentScript.displayName.text = name; // << Set the comment text
            commentScript.comment.text = text; // << Set the comment's display name

            //This prevents the comment from fading in before the rest of the UI if all the chat UI is meant to be fading in. Otherwise it makes the comment fade in at the bottom of the list
            if (isLoadingCommentsFirstTime == false)
            {
                commentScript.fadeInGroup.MoveForward();
            }

            //This adds all of the created UI elements to the allChatUI lists so they will fade out with the rest of the Chat UI
            if (allChatUI.movesImages == null)
            {
                allChatUI.movesImages = new System.Collections.Generic.List<DCP_UIMove1DImage>();
            }
            allChatUI.movesImages.Add(commentScript.thisMoveImage);
            if (allChatUI.movesTexts == null)
            {
                allChatUI.movesTexts = new System.Collections.Generic.List<DCP_UIMove1DText>();
            }
            allChatUI.movesTexts.Add(commentScript.commentMoveText);
            allChatUI.movesTexts.Add(commentScript.displayMoveText);
        }
    }

    //This IEnumerator runs the Command Sequence to submit the players comment to the chat
    IEnumerator SendComment (string name, string text)
    {
        //Run the command sequence called 'Post Comment' on the database name which has been retrieved in the awake method. Sends the display name and comment text (from the ui input fields) as parameters
        IEnumerator e = DCP.RunCS(databaseName, "Post Comment", new string[2] { name, text });

        //wait for the command sequence to finish loading
        while (e.MoveNext())
        {
            yield return e.Current;
        }

        //For this command sequence we will assume the result is a success

        //As we have just sent a comment we should update the list of comments by forcing a Refresh early
        //We don't add the comment to our ui list now as it might not be in the same order as the database (it could also cause a double post of a comment with another comment missing)
        ForceRefresh();
    }

    //This IEnumerator runs the Command Sequence to get the data for all of the comments in the database
    IEnumerator GetChatInfo()
    {
        //Run the command sequence called 'Get Chat Data' on the database name which has been retrieved in the awake method. We don't need to include a string[] as the third parameter as the sequence has no input variables
        IEnumerator e = DCP.RunCS(databaseName, "Get Chat Data");

        //wait for the command sequence to finish loading
        while (e.MoveNext())
        {
            yield return e.Current;
        }

        //get the command sequence result
        string returnText = e.Current as string;

        if (isLoggedIn == true)
        {
            /*
            The sequence that is returned will be in the form
            Success//Comment1//Comment2//Comment3//...

            where each Comment is in the form:
            DisplayName/CommentText
            */
            
            //Split the data by '//'
            string[] splitComments = returnText.Split(new string[1] { "//" }, System.StringSplitOptions.None);

            //Check if the data starts with 'Success'
            bool isSuccess = false;
            if (splitComments != null)
            {
                if (splitComments.Length > 0)
                {
                    if (splitComments[0] == "Success")
                    {
                        isSuccess = true;
                    }
                }
            }

            if (isSuccess == true)
            {
                //The data was successfully retrieved
                if (isLoadingCommentsFirstTime == true)
                {
                    //The player has just joined the chat
                    for (int i = 1; i < splitComments.Length; i++)
                    {
                        //Split the comment's data by '/'. If it gives two strings (display name and comment text) then it is valid so add the comment
                        string[] commentData = splitComments[i].Split(new string[1] { "/" }, System.StringSplitOptions.None);
                        if (commentData.Length == 2)
                        {
                            AddComment(commentData[0], commentData[1]);
                        }
                    }
                } else
                {
                    //The chat is refreshing
                    int commentsFound = 0;
                    for (int i = 1; i < splitComments.Length; i++)
                    {
                        string[] commentData = splitComments[i].Split(new string[1] { "/" }, System.StringSplitOptions.None);
                        if (commentData.Length == 2)
                        {
                            //comment is valid
                            if (commentsFound < comments.Count) // << If we have already created this comment on a previous refresh
                            {
                                //This comment has been already been loaded
                            } else
                            {
                                //This comment has not been loaded
                                AddComment(commentData[0], commentData[1]); //Add the comment
                            }
                            commentsFound++;
                        }
                    }
                }
            }

            //For this command sequence we will ignore the case of there being an error as is will just leave the chat blank which suggests there is an error

            //Whether it is joining the chat or refreshing, it needs to start the refreshing delay in either case
            StartRefreshDelay();

            //If the player has just joined the chat and it is loading the comments for the first time make all of the UI fade in
            if (isLoadingCommentsFirstTime == true)
            {
                loadingGroup.MoveBack();
                chatUIIn.StartDelay();
                isLoadingCommentsFirstTime = false;
            }

            //Disables the 'Sending...' text if it is refreshing after the player has just submitted a comment
            sendingCommentStuff.gameObject.SetActive(false);
            postCommentStuff.gameObject.SetActive(true);
        }
    }

    void StartRefreshDelay ()
    {
        //runs the coroutine to start the refresh countdown
        StartCoroutine(CountDown(refreshNumber));
    }

    IEnumerator CountDown (int thisRefresh)
    {
        refreshText.text = "Refreshing in ... 5"; // << Changes the UI text to show the number of seconds left
        yield return new WaitForSeconds(1); // << a 1 second delay
        if (thisRefresh == refreshNumber) // << These if statements make sure this countdown has not been cancelled by the player submitting a post or logging out
        {
            refreshText.text = "Refreshing in ... 4";
            yield return new WaitForSeconds(1);
            if (thisRefresh == refreshNumber)
            {
                refreshText.text = "Refreshing in ... 3";
                yield return new WaitForSeconds(1);
                if (thisRefresh == refreshNumber)
                {
                    refreshText.text = "Refreshing in ... 2";
                    yield return new WaitForSeconds(1);
                    if (thisRefresh == refreshNumber)
                    {
                        refreshText.text = "Refreshing in ... 1";
                        yield return new WaitForSeconds(1);
                        if (thisRefresh == refreshNumber)
                        {
                            //the refresh has not been cancelled
                            refreshText.text = "Refreshing ...";
                            StartCoroutine(GetChatInfo()); // << Run the command sequence to get the chat comments and update the UI
                            refreshNumber++;
                        }
                    }
                }
            }
        }
    }

    void ForceRefresh ()
    {
        //This is called when the player submits a comment to run a refresh immediatley
        refreshText.text = "Refreshing ...";
        StartCoroutine(GetChatInfo()); // << runs the ienumerator which runs the command sequence to get the chat data
        refreshNumber++; // << prevents the normal refresh (in 5 second cycle) from working. It will be started again by the GetChatInfo ienumerator once the Command Sequence has finished
    }

}
