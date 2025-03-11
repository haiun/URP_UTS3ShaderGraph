# URP_UTS3ShaderGraph
이 프로젝트는 Unity엔진에서 NPR(Non-Photo Realistic)렌더링 셰이더로 유명한 UTS3(Unity-Chan Toon Shader 3)의 일부 기능을 임의로 Shader Graph로 컨버팅하고 최적화를 진행하였습니다.

UTS3에 다양한 기능 지원은 Shader특성때문에 사용되지 않더라도 모든 계산을 진행합니다.
UTS3를 사용해서 아트워크를 구성한 후 사용된 기능만 추출해서 정리만 하더라도 최적화가 가능합니다.
그리고 Shader Graph를 사용할 때에는 코드를 추가하지 않고 추가적인 기능추가도 가능합니다.

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

## 결과

