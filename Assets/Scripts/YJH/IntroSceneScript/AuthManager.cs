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
    public static FirebaseUser user; //인증된 유저 정보 기억
    public FirebaseAuth auth; //인증 진행을 위한 정보

    public InputField emailField;
    public InputField pwField;
    public InputField nickField;

    public InputField registerEM;
    public InputField registerPW;
    public InputField verifyPW;

    public Text warningText;
    public Text warningText2;
    public Text successText;

    public static DatabaseReference dbRef; //DB용 레퍼런스

    public Button registerBtn;
    public Button loginBtn;
    public Button creatBtn;
    public GameObject registerPanel;
    public GameObject nickNamePanel; // 닉네임 입력창 패널
    public Button saveNickBtn; // 닉네임 저장 버튼

    public GameObject loadingPanel;// 로딩 판넬
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
                Debug.Log("파이어베이스 문제있음");
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
            Debug.LogWarning("로그인 실패 :" + loginTask.Exception);

            FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode; // 좀 더 편하게 보려고 열거형으로 받아서 봄

            string message = "";

            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "이메일 누락되었습니다";
                    break;
                case AuthError.MissingPassword:
                    message = "비밀번호가 누락되었습니다";
                    break;
                case AuthError.WrongPassword:
                    message = "비밀번호가 틀렸습니다";
                    break;
                case AuthError.UserNotFound:
                    message = "아이디가 존재하지 않습니다";
                    break;
                case AuthError.InvalidEmail:
                    message = "이메일 형태가 맞지 않습니다";
                    break;
                default:
                    message = "관리자에게 문의 바랍니다";
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
                // 닉네임이 없는 경우 닉네임 입력 필드 활성화
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
            warningText2.text = "확인 비밀번호가 다릅니다.";
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
                    message = "이메일이 누락되었습니다.";
                    break;
                case AuthError.MissingPassword:
                    message = "비밀번호가 누락되었습니다.";
                    break;
                case AuthError.WeakPassword:
                    message = "비밀번호는 6자 이상 입력해주세요";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "중복된 이메일 입니다.";
                    break;
                default:
                    message = "관리자에게 문의 바랍니다";
                    break;
            }
            warningText2.text = message;
            yield break;
        }
        if (registerTask.Result != null)
        {
            user = registerTask.Result.User;
            warningText2.text = ""; // 회원가입 성공 시 오류 메시지 초기화
            successText.text = "계정 생성이 완료되었습니다!";
        }

    }
    public void SaveNickName()
    {
        string newNick = nickField.text;

        if (string.IsNullOrEmpty(newNick))
        {
            warningText.text = "닉네임을 입력해주세요.";
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
            warningText.text = "닉네임 저장 실패.";
        }
        else
        {
            warningText.text = "";
            nickNamePanel.SetActive(false); // 닉네임 입력창 닫기

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

