# League of Legends Arena 모작 (AOS 네트워크 팀 게임)

Photon 기반의 4인 매칭 시스템과 팀 나누기,  
상태패턴 기반 캐릭터 제어 및 셰이더 연출이 결합된  
**네트워크 팀전 게임 프로젝트**

---

## 📌 프로젝트 개요

| 항목     | 내용                          |
|----------|-------------------------------|
| 유형     | 네트워크 팀 프로젝트 (AOS형)   |
| 기간     | 2025.03.17 ~ 2025.04.03 (14일) |
| 인원     | 개발자 4명                     |
| 도구     | Unity, Photon PUN2, Amplify Shader |

---

## 🎮 Contributors 핵심 구현 기능

## jonghyun109 
###  매칭 시스템
- **Photon Callbacks**를 활용한 **4인 랜덤 매칭**
- **마스터 클라이언트 기준으로 팀 나누기 구현**
- ActorNumber 기반 팀 구성 → RPC로 팀 정보 동기화

###  챔피언 제작
- 상태 패턴(State Pattern) 기반 캐릭터 구성
  → 이동, 공격, 스킬 사용 등 상태별로 분리하여 확장성 확보
- **공통 인터페이스 기반**으로 다양한 챔피언 제작 가능

### 🌑 기타 시스템
- **셰이더 활용 암흑 시야 구현** (Fog of War 개념)
- 아웃게임 UI → 인게임까지 이어지는 **전체 게임 흐름 설계 및 구현**
---

## 🛠️ 기술 스택

- **Unity 2022.3 (URP)**
- **C#**
- **Photon PUN2 (멀티플레이어 기능)**
- **Amplify Shader Editor (시야 제한 셰이더 제작)**

---

## ▶ 실행 방법

```bash
git clone https://github.com/jonghyun109/YourRepoName.git
