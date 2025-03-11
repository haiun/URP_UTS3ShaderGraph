# URP_UTS3ShaderGraph
이 프로젝트는 Unity엔진에서 NPR(Non-Photo Realistic)렌더링 셰이더로 유명한 UTS3(Unity Toon Shader)의 일부 기능을 임의로 Shader Graph로 컨버팅하고 최적화를 진행하였습니다.<br>
UTS3 링크 : https://docs.unity3d.com/Packages/com.unity.toonshader@0.11/manual/index.html <br>
<br>

UTS3는 다양한 기능 지원하고 각 기능들의 계산 결과를 마지막에 선별하는 Shader특성 때문에 사용되지 않더라도 모든 계산을 진행합니다.<br>
UTS3를 사용해서 아트워크를 구성한 후 사용된 기능만 추출해서 정리만 하더라도 최적화가 가능합니다.<br>
그리고 Shader Graph를 사용하면 코드를 없이 기능추가도 가능합니다.<br>
<br>

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/shadergraph.png?raw=true"/>

작업한 Shader Graph의 경로는 Assets/UTS3_opt/Shader/UTS3_opt.shadergraph 입니다.<br>
이후 Shader Graph버전의 명칭을 OPT라고 명시하겠습니다.<br>
<br>
임의로 캐릭터를 꾸며보고 여기에 사용된 기능들과 환경을 특정해서 최대한 Shader Graph로 옮겼습니다.<br>
일부 기능들은 Shader Graph에서 구현이 불가능하기 때문에, hlsl로 작성되었습니다. (그림자맵, 광원색상)<br>

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



(좌 UTS3 / 우 OPT)

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/comp_no_shadow.gif?raw=true"/>
<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/comp_shadow.gif?raw=true"/>

이미지가 완전히 같습니다.

## 성능 비교

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/speed_test.png?raw=true"/>

Assets/Scenes/Scene_UTS3_Massive 씬과 Assets/Scenes/Scene_Opt_Massive를 실행해서 Rendering Debugger로 실행속도와 Frame Debugger로 SRP Batch구조를 비교합니다.
UnityEditor/4K UHD 해상도에 배치상태가 동일하고 DrawOpaqueObjects의 SRP Batch가 17회로 완전히 동일한 환경에서 Shader교체하면서 테스트했습니다.

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/srp_batch_17.gif?raw=true"/>

### 실행 속도

Rendering Debugger / Display Stats에서 DrawOpaqueObjects CPU/GPU 항목에서 유의미한 수치 변화가 있었습니다.

| Case | 그림자OFF CPU | 그림자OFF GPU | 그림자ON CPU | 그림자ON GPU |
| ------ | ------ | ------| ------ | ------ |
| UTS3 | 2.02~2.10 ms | 6.51~6.60 ms | 2.02~2.10 ms | 6.54~6.60 ms |
| OPT | 1.39~1.42 ms | 2.81~2.90 ms | 1.48~1.50 ms | 4.59~4.70 ms |
| 속도향상 | +46.6% | +132.5% | +38.3% | +44.3% |

Frame Debugger으로 확인한 상수버퍼의 float갯수가 4624개(float320개, vector4 4192개, Matrix4x4 112개)에서 188개로 아주 많이 줄었습니다.
Forward+에서 사용하기 위한 AdditionalLight관련 값들이 가장 많았고 이외에는 URP의 ReflProbes관련 값이 많았습니다.

## 결과

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-005.png?raw=true"/>

완전히 동일한 이미지를 약 40% 이상의 속도향상과 함깨 입력해야하는 항목이 간소해져서 관리가 쉬워졌습니다.
Shader Graph화를 통해 확장이 용이해졌습니다.


## 외각선 보완


<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/simple_outline.gif?raw=true"/>
<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/simple_outline_shadow.gif?raw=true"/>


NPR에서 외각선 사용여부에 따라 화면의 느낌이 많이 바뀝니다.


하지만 UTS3에서 외각선을 사용하면 SRP Batch가 무력화되어 속도가 심각하게 느려집니다.

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/srp_batch_failed_outline.gif?raw=true"/>

| Case | 그림자OFF CPU | 그림자OFF GPU | 그림자ON CPU | 그림자ON GPU |
| ------ | ------ | ------| ------ | ------ |
| UTS3 중앙값 | 2.06 ms | 6.55 ms | 2.06 ms | 6.57 ms |
| UTS3+외각선 | 73 ms | 37 ms | 74 ms | 40 ms |

프로젝트 주제였던 Shader Graph는 Multi Pass를 지원하지 않는다고 알고있습니다.
Universal Renderer Data에서 Render Objects나 MaterialPropertyBlock을 사용하는 방법도 어울리지 않아 간단한 단색 외각선 Shader Graph를 작성하여 런타임에 외각선 매쉬를 직접 생성하는 테스트를 추가로 진행했습니다.

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/srp_batch_outline.gif?raw=true"/>
