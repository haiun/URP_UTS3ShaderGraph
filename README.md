# URP_UTS3ShaderGraph
이 프로젝트는 Unity엔진에서 NPR(Non-Photo Realistic)렌더링 셰이더로 유명한 UTS3(Unity Toon Shader)의 일부 기능을 임의로 Shader Graph로 컨버팅하고 최적화를 진행하였습니다.
UTS3 링크 : https://docs.unity3d.com/Packages/com.unity.toonshader@0.11/manual/index.html

UTS3는 다양한 기능 지원하고 각 기능들의 계산 결과를 마지막에 선별하는 Shader특성 때문에 사용되지 않더라도 모든 계산을 진행합니다.
UTS3를 사용해서 아트워크를 구성한 후 사용된 기능만 추출해서 정리만 하더라도 최적화가 가능합니다.
그리고 Shader Graph를 사용하면 코드를 없이 기능추가도 가능합니다.

작업한 Shader Graph의 경로는 Assets/UTS3_opt/Shader/UTS3_opt.shadergraph 입니다.

임의로 캐릭터를 꾸며보고 여기에 사용된 기능들과 환경을 특정해서 최대한 Shader Graph로 옮겼습니다.
일부 기능들은 Shader Graph에서 구현이 불가능하기 때문에, hlsl로 작성되었습니다. (그림자맵, 광원색상)

<details>
  <summary>작업 환경 & 사용한 기능 & 제외한 기능 (자세히..)</summary>
  
작업 환경 : Unity6 (6000.0.41f1), URP, 포워드렌더링, Cascade Shadow Map, 메인광원1개

사용한 기능
- Three Color Map and Control Map Settings
  - Base Map
  - Normal Map
- Shading Steps and Feather Settings
  - Base Color Step & Feather
  - Shading Color Step & Feather
- Highlight Settings
  - Highlight Power
  - Specular Mode Soft Only
- Rim Light
  - Color & Level

제외한 기능
- Outline Settings
- Material Capture Settings
- Emission Settings
- Angel Ring Projection Settings
- Metaverse Settings
</details>

## UTS3과 렌더링 비교


## 성능 비교

Assets/Scenes/Scene_UTS3_Massive 씬과 Assets/Scenes/Scene_Opt_Massive를 실행해서 Rendering Debugger로 실행속도와 Frame Debugger로 SRP Batch구조를 비교합니다.
4k해상도에 배치상태가 동일하고 DrawOpaqueObjects의 SRP Batch가 17회로 완전히 똑같은 환경에서 Shader교체하면서 테스트했습니다.

### 실행 속도

### SRP Batch

## 결과

## 보완
