# URP_UTS3ShaderGraph
이 프로젝트는 Unity엔진에서 NPR(Non-Photo Realistic)렌더링 셰이더로 유명한 UTS3(Unity Toon Shader)의 일부 기능을 임의로 Shader Graph로 변환하고 최적화를 진행하였습니다.<br>
UTS3 링크 : https://docs.unity3d.com/Packages/com.unity.toonshader@0.11/manual/index.html <br>
<br>

UTS3는 다양한 기능 지원하고 각 기능들의 계산 결과를 마지막에 선별하는 Shader특성 때문에 사용되지 않더라도 모든 계산을 진행합니다.<br>
UTS3를 사용해서 아트워크를 구성한 후 사용된 기능만 추출해서 정리만 하더라도 최적화가 가능합니다.<br>
그리고 Shader Graph를 사용하면 코드 수정 없이 기능 추가도 가능하며 협업 시 커뮤니케이션에 활용하면 도움이 됩니다.<br>
<br>

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/shadergraph.png?raw=true"/>

작업한 Shader Graph의 경로는 'Assets/UTS3_opt/Shader/UTS3_opt.shadergraph' 입니다.<br>
이후 Shader Graph버전의 명칭을 OPT라고 명시하겠습니다.<br>
<br>
임의로 캐릭터를 꾸며보고 여기에 사용된 기능들과 환경을 특정해서 최대한 Shader Graph로 옮겼습니다.<br>
일부 기능들은 Shader Graph에서 구현이 불가능하기 때문에, hlsl로 작성되었습니다. (그림자감쇠값, 광원색상)<br>

<details>
  <summary>작업 환경 & 사용한 기능 & 제외한 기능 (자세히..)</summary>
  
작업 환경 : Unity6 (6000.0.41f1), URP, 포워드렌더링, Cascade Shadow Map, 메인광원1개<br>

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
- Outline Settings (Shader Graph가 Multi Pass를 지원하지 않음 / 글의 마지막에 보완함)
- Material Capture Settings
- Emission Settings
- Angel Ring Projection Settings
- Metaverse Settings
</details>

## UTS3과 렌더링 비교


두 Shader의 결과물이 동일한지 비교한 결과입니다.
(좌 UTS3 / 우 OPT)<br>

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/comp_no_shadow.gif?raw=true"/>
<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/comp_shadow.gif?raw=true"/>

## 성능 비교

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/speed_test.png?raw=true"/>

'Assets/Scenes/Scene_UTS3_Massive.unity'와 'Assets/Scenes/Scene_Opt_Massive.unity'를 실행해서 Rendering Debugger로 실행속도와 Frame Debugger로 SRP Batch구조를 비교합니다.<br>
DrawOpaqueObjects의 SRP Batch가 17회로 완전히 동일함을 확인 후 Shader가 병목지점이 될 수 있도록 4K UHD 해상도로 만들어서 11x11개의 캐릭터 렌더링에 대한 프로파일링 테스트를 했습니다.<br>

[링크 - 웹에서 테스트 배치 보기](https://haiun.github.io/UnityChan_TEST/ "WebGl버전 실행")<br>

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/srp_batch_17.gif?raw=true"/>

### 실행 속도

Rendering Debugger / Display Stats에서 DrawOpaqueObjects CPU/GPU 항목에서 유의미한 수치 변화가 있었습니다.<br>

| Case | 그림자OFF CPU | 그림자OFF GPU | 그림자ON CPU | 그림자ON GPU |
| ------ | ------ | ------| ------ | ------ |
| UTS3 | 2.02~2.10 ms | 6.51~6.60 ms | 2.02~2.10 ms | 6.54~6.60 ms |
| OPT | 1.39~1.42 ms | 2.81~2.90 ms | 1.48~1.50 ms | 4.59~4.70 ms |
| 속도향상 | +46.6% | +132.5% | +38.3% | +44.3% |

Frame Debugger으로 확인한 상수버퍼의 float갯수가 4624개(float 320개, vector4 4192개, Matrix4x4 112개)에서 188개로 아주 많이 줄었습니다.<br>
Forward+에서 사용하기 위한 최대 256개의 AdditionalLight관련 값들이 배열로 등록 되어 있어서 가장 많았고 이외에는 URP의 최대 64개의 ReflProbes관련 배열이 크기가 컷습니다.<br>

## 결과

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-005.png?raw=true"/>

완전히 동일한 이미지를 약 40% 이상의 속도향상과 함깨 입력해야하는 항목이 간소해져서 관리가 쉬워졌습니다.<br>
Shader Graph화를 통해 확장이 용이해졌습니다.<br>

UTS3과 Shader Property를 동일하게 사용하기 때문에 기존 UTS3Shader에서 사용되던 Material Property를 복사하여 OPS로 쉽게 이식됩니다.<br>

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-008.png?raw=true"/>

부가적으로 메인 조명 1개만 사용하여 플랫폼 제한이 확장되어 FireFox에서만 실행되던 WebGL빌드가 크롬/엣지 브라우저에서도 실행됩니다.<br>


## 외각선 보완




NPR에서 외각선 사용여부에 따라 화면의 느낌이 많이 바뀝니다.<br>

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-015.png?raw=true"/>
<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-012.png?raw=true"/>
<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-017.png?raw=true"/>
<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-013.png?raw=true"/>

하지만 UTS3에서 외각선이 Multi Pass Rendering으로 구현되어 있기 때문에 비슷한 Material을 연속해서 그리는 것이 중요한 SRP Batch의 최적화가 무력화되어 횟수가 4115번으로 늘어나면서 속도가 심각하게 느려지는 현상을 확인했습니다.<br>

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/srp_batch_failed_outline.gif?raw=true"/>

| Case | 그림자OFF CPU | 그림자OFF GPU | 그림자ON CPU | 그림자ON GPU |
| ------ | ------ | ------| ------ | ------ |
| UTS3 중앙값 | 2.06 ms | 6.55 ms | 2.06 ms | 6.57 ms |
| UTS3+외각선 | 73 ms | 37 ms | 74 ms | 40 ms |

Universal Renderer Data에서 Render Objects나 MaterialPropertyBlock을 사용하는 방법도 어울리지 않아 간단한 단색 외각선 Shader Graph를 작성하여 런타임에 외각선 매쉬를 직접 생성하는 추가 테스트를 진행했습니다.<br>
이후 외각선용 Shader Graph의 MeshBackfaceOutline명시하며 내용은 아래와 같습니다.<br>

### Shader Graph

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-006.png?raw=true"/>

### 렌더링

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/simple_outline.gif?raw=true"/>
<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/simple_outline_shadow.gif?raw=true"/>

### 성능 측정

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/srp_batch_outline.gif?raw=true"/>

Multi Pass Rendering이 아닌 일반적인 다른 오브젝트로 취급되자 SRP Batch가 26회 진행됨을 확인했고 Skinning연산이 2배 증가했습니다.<br>
물리적으로 오브젝트가 약 2배가량 늘었기 때문에 모든 수행시간이 어느 정도 비례하여 시간이 소요될 것으로 추측했습니다.<br>
테스트 결과도 예상과 비슷했습니다.<br>

DrawOpaqueObjects CPU/GPU 항목에서 측정한 시간은 아래와 같습니다.

| Case | 그림자OFF CPU | 그림자OFF GPU | 그림자ON CPU | 그림자ON GPU |
| ------ | ------ | ------| ------ | ------ |
| UTS3+MeshBackfaceOutline | 4.31~4.51 ms | 5.88~5.91 ms | 4.26~4.63 ms | 5.82~5.89 ms |
| OPT+MeshBackfaceOutline | 3.42~3.60ms | 3.12~3.35 ms | 3.53~3.73 ms | 3.18~3.31 ms |

수치 비교는 DrawOpaqueObjects항목만 했지만, 그림자맵을 포함 다른 여러 부분에서 CPU부하가 비례해서 약 2배 늘어났습니다.<br>
외각선이 없이 측정했던 수치와 비교해서 테스트환경은 GPU병목에서 CPU병목현상으로 전환됨을 확인했습니다.<br>
