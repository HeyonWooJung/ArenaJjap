using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class AuthManager : MonoBehaviour
{
    public Button startButton;

    public static FirebaseUser user; //������ ���� ���� ���
    public FirebaseAuth auth; //���� ������ ���� ����

    public InputField emailField;
    public InputField pwField;
    public InputField nickField;

    public InputField registerEM;
    public InputField registerPW;
    public InputField verifyPW;

    public Text warningText;

    public static DatabaseReference dbRef; //DB�� ���۷���

    public Button registerBtn;
    public Button loginBtn;
    public GameObject registerPanel;
    public void OnRegisterPanel()
    {
        registerPanel.SetActive(true);
    }

    private void Awake()
    {
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

        startButton.interactable = false;
        warningText.text = "";
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

        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogWarning("������ ���� ������ �α��� ���� :" + loginTask.Exception);

            //���̾�̽� ���� ������ �м��� �� �ִ� ������ ����
            FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode; // �� �� ���ϰ� ������ ���������� �޾Ƽ� ��

            string message = "";

            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "�̸��� ����";
                    break;
                case AuthError.MissingPassword:
                    message = "�н����� ����";
                    break;
                case AuthError.WrongPassword:
                    message = "�н����� Ʋ��";
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
            user = loginTask.Result.User; //������ ������ ����ϰ� �ְ� ��
            warningText.text = "";
            nickField.text = user.DisplayName; //������ �� �г��� �����ͼ� ��ǲ�ʵ忡 ������ �Ѹ�����
            startButton.interactable = true;
        }
    }
    IEnumerator RegisterCor(string email, string pw, string verPw)
    {
        
        if(pw != verPw)
        {
        }        
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, pw);

        yield return new WaitUntil(predicate: () => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            FirebaseException firebaseEx = registerTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "";

            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "�̸��� ����";
                    break;
                case AuthError.MissingPassword:
                    message = "�н����� ����";
                    break;
                case AuthError.WeakPassword:
                    message = "��й�ȣ�� 6�� �̻� �Է����ּ���";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "�ߺ� �̸���";
                    break;
                default:
                    message = "�����ڿ��� ���� �ٶ��ϴ�";
                    break;
            }

            warningText.text = message;
        }
        //else
        //{
        //    user = registerTask.Result.User;

        //    if (user != null)
        //    {
        //        UserProfile profile = new UserProfile { DisplayName = "geust" };// �ӽ� ������ ����

        //        var profileTask = user.UpdateUserProfileAsync(profile); //�ۼ��� ������ �������� ���� ��������

        //        yield return new WaitUntil(predicate: () => profileTask.IsCompleted);

        //        if (profileTask.Exception != null)
        //        {
        //            FirebaseException firebaseEx = profileTask.Exception.GetBaseException() as FirebaseException;
        //            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
        //            warningText.text = "�г��� ��� ����";
        //        }
        //        else
        //        {
        //            warningText.text = "";
        //            successText.text = "���� �Ϸ�. �ݰ����ϴ�" + user.DisplayName + "��";
        //            startButton.interactable = true;
        //        }
        //    }
        //}


        

    }
}

