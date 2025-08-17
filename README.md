# DCommando
## 문서
- [개발일지](/DevelopmentLog_7_8.md)

### 계층 구조
```
Root
 ┣ Core                // 이벤트, 상태머신, DI, 유틸
 ┣ Systems             // 게임 도메인 시스템(전투, 퀘스트, 인벤토리 등)
 ┃ ┣ ...
 ┣ Policies            // 정책 도메인
 ┣ Models              // 도메인 데이터 모델
 ┣ Services            // 네트워크, 저장, SDK 연동
 ┣ UI                  // Unity View 계층
 ┣ ViewModels          // UI <-> 도메인 상태 연결
 ┗ Data                // ScriptableObject, Config, DB 모델
```
### 참조 방향
```mermaid
graph TB
    subgraph "계층"
        UI[UI<br/>Unity View 계층]
        VM[ViewModel<br/>UI ↔ 도메인 연결]
        SYS[Systems<br/>게임 도메인 시스템]
        POL[Policies<br/>비즈니스 규칙]
        MOD[Models<br/>도메인 데이터]
        SER[Services<br/>외부 서비스 연동]
        DAT[Data<br/>설정 및 저장 데이터]
        COR[Core<br/>공통 인프라]

        Memo[모든 계층에서 참조 가능]
    end
    COR --> Memo

    UI --> VM
    VM --> SYS
    VM --> SER
    VM --> MOD
    SYS --> MOD
    SYS --> POL
    SYS --> SER
    SYS --> DAT
    SER --> DAT
    SER --> POL
    MOD --> DAT
```

---
# OpenSource
- [UniTask](https://github.com/Cysharp/UniTask) - 유니티 쓰레드 관리
- [R3](https://github.com/Cysharp/R3) - 리액티브 프로그래밍
- [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676) - 연출
- [Zenject](https://github.com/modesttree/Zenject?tab=readme-ov-file#installation-) - 의존성 주입
- [NSubstitute](https://github.com/Thundernerd/Unity3D-NSubstitute) - 테스트 용으로 사용
