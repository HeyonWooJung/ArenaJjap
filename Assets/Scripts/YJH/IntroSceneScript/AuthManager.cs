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

    public static FirebaseUser user; //인증된 유저 정보 기억
    public FirebaseAuth auth; //인증 진행을 위한 정보

    public InputField emailField;
    public InputField pwField;
    public InputField nickField;

    public InputField registerEM;
    public InputField registerPW;
    public InputField verifyPW;

    public Text warningText;

    public static DatabaseReference dbRef; //DB용 레퍼런스

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
                Debug.Log("파이어베이스 문제있음");
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
            Debug.LogWarning("다음과 같은 이유로 로그인 실패 :" + loginTask.Exception);

            //파이어베이스 에선 에러를 분석할 수 있는 형식을 제공
            FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode; // 좀 더 편하게 보려고 열거형으로 받아서 봄

            string message = "";

            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "이메일 누락";
                    break;
                case AuthError.MissingPassword:
                    message = "패스워드 누락";
                    break;
                case AuthError.WrongPassword:
                    message = "패스워드 틀림";
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
            user = loginTask.Result.User; //인증된 정보를 기억하고 있게 함
            warningText.text = "";
            nickField.text = user.DisplayName; //서버의 내 닉네임 가져와서 인풋필드에 역으로 뿌릴거임
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
                    message = "이메일 누락";
                    break;
                case AuthError.MissingPassword:
                    message = "패스워드 누락";
                    break;
                case AuthError.WeakPassword:
                    message = "비밀번호는 6자 이상 입력해주세요";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "중복 이메일";
                    break;
                default:
                    message = "관리자에게 문의 바랍니다";
                    break;
            }

            warningText.text = message;
        }
        //else
        //{
        //    user = registerTask.Result.User;

        //    if (user != null)
        //    {
        //        UserProfile profile = new UserProfile { DisplayName = "geust" };// 임시 프로필 생성

        //        var profileTask = user.UpdateUserProfileAsync(profile); //작성한 프로필 유저에게 세팅 보내버림

        //        yield return new WaitUntil(predicate: () => profileTask.IsCompleted);

        //        if (profileTask.Exception != null)
        //        {
        //            FirebaseException firebaseEx = profileTask.Exception.GetBaseException() as FirebaseException;
        //            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
        //            warningText.text = "닉네임 사용 실패";
        //        }
        //        else
        //        {
        //            warningText.text = "";
        //            successText.text = "생성 완료. 반갑습니다" + user.DisplayName + "님";
        //            startButton.interactable = true;
        //        }
        //    }
        //}


        

    }
}

