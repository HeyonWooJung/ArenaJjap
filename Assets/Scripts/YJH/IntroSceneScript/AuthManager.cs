using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class AuthManager : MonoBehaviourPunCallbacks
{
    public static FirebaseUser user; //������ ���� ���� ���
    public FirebaseAuth auth; //���� ������ ���� ����

    public InputField emailField;
    public InputField pwField;
    public InputField nickField;

    public InputField registerEM;
    public InputField registerPW;
    public InputField verifyPW;

    public Text warningText;
    public Text warningText2;
    public Text successText;

    public static DatabaseReference dbRef; //DB�� ���۷���

    public Button registerBtn;
    public Button loginBtn;
    public Button creatBtn;
    public GameObject registerPanel;
    public GameObject nickNamePanel; // �г��� �Է�â �г�
    public Button saveNickBtn; // �г��� ���� ��ư

    public GameObject loadingPanel;// �ε� �ǳ�
    public GameObject loadingSpinner;
    public void OnRegisterPanel()
    {
        registerPanel.SetActive(true);
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;

            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                Debug.Log("���̾�̽� ��������");
            }
        });
    }
    void Start()
    {
        registerPanel.SetActive(false);
        nickNamePanel.SetActive(false);
        loadingPanel.SetActive(false);

        warningText.text = "";
        warningText2.text = "";
    }
    private void Update()
    {
        loadingSpinner.transform.Rotate(0, 0, -200 * Time.deltaTime);
    }
    public void Login()
    {
        StartCoroutine(LoginCor(emailField.text, pwField.text));
    }

    public void Register()
    {
        StartCoroutine(RegisterCor(registerEM.text, registerPW.text, verifyPW.text));
    }

    IEnumerator LoginCor(string email, string pw)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, pw);
        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogWarning("�α��� ���� :" + loginTask.Exception);

            FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode; // �� �� ���ϰ� ������ ���������� �޾Ƽ� ��

            string message = "";

            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "�̸��� �����Ǿ����ϴ�";
                    break;
                case AuthError.MissingPassword:
                    message = "��й�ȣ�� �����Ǿ����ϴ�";
                    break;
                case AuthError.WrongPassword:
                    message = "��й�ȣ�� Ʋ�Ƚ��ϴ�";
                    break;
                case AuthError.UserNotFound:
                    message = "���̵� �������� �ʽ��ϴ�";
                    break;
                case AuthError.InvalidEmail:
                    message = "�̸��� ���°� ���� �ʽ��ϴ�";
                    break;
                default:
                    message = "�����ڿ��� ���� �ٶ��ϴ�";
                    break;
            }
            warningText.text = message;
        }
        else
        {
            user = loginTask.Result.User;
            warningText.text = "";
            if (string.IsNullOrEmpty(user.DisplayName))
            {
                // �г����� ���� ��� �г��� �Է� �ʵ� Ȱ��ȭ
                nickNamePanel.SetActive(true);
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();

                StartCoroutine(ConnectServer());
            }
        }


    }
    IEnumerator RegisterCor(string email, string pw, string verPw)
    {
        if (pw != verPw)
        {
            warningText2.text = "Ȯ�� ��й�ȣ�� �ٸ��ϴ�.";
            yield break;
        }

        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, pw);
        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            FirebaseException firebaseEx = registerTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "";

            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "�̸����� �����Ǿ����ϴ�.";
                    break;
                case AuthError.MissingPassword:
                    message = "��й�ȣ�� �����Ǿ����ϴ�.";
                    break;
                case AuthError.WeakPassword:
                    message = "��й�ȣ�� 6�� �̻� �Է����ּ���";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "�ߺ��� �̸��� �Դϴ�.";
                    break;
                default:
                    message = "�����ڿ��� ���� �ٶ��ϴ�";
                    break;
            }
            warningText2.text = message;
            yield break;
        }
        if (registerTask.Result != null)
        {
            user = registerTask.Result.User;
            warningText2.text = ""; // ȸ������ ���� �� ���� �޽��� �ʱ�ȭ
            successText.text = "���� ������ �Ϸ�Ǿ����ϴ�!";
        }

    }
    public void SaveNickName()
    {
        string newNick = nickField.text;

        if (string.IsNullOrEmpty(newNick))
        {
            warningText.text = "�г����� �Է����ּ���.";
            return;
        }

        UserProfile profile = new UserProfile { DisplayName = newNick };
        var profileTask = user.UpdateUserProfileAsync(profile);

        StartCoroutine(WaitForNickUpdate(profileTask));
    }

    IEnumerator WaitForNickUpdate(System.Threading.Tasks.Task profileTask)
    {
        yield return new WaitUntil(() => profileTask.IsCompleted);

        if (profileTask.Exception != null)
        {
            warningText.text = "�г��� ���� ����.";
        }
        else
        {
            warningText.text = "";
            nickNamePanel.SetActive(false); // �г��� �Է�â �ݱ�

            PhotonNetwork.ConnectUsingSettings();
            StartCoroutine(ConnectServer());
        }
    }
    public override void OnConnectedToMaster()
    {
        if (AuthManager.user != null)
        {
            PhotonNetwork.NickName = AuthManager.user.DisplayName;
            SceneManager.LoadScene("Scene2");
        }
    }
    IEnumerator ConnectServer()
    {
        loadingPanel.SetActive(true);

        yield return new WaitUntil(() => PhotonNetwork.IsConnected);        
    }
}

