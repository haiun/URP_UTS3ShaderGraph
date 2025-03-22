# URP_UTS3ShaderGraph


<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-012.png?raw=true"/>
<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-013.png?raw=true"/>

이 프로젝트는 Unity엔진에서 NPR(Non-Photo Realistic)렌더링 Shader로 유명한 UTS3(Unity Toon Shader)의 일부 기능을 Shader Graph로 변환하고 최적화를 진행하였습니다.<br>
UTS3 링크 : https://docs.unity3d.com/Packages/com.unity.toonshader@0.11/manual/index.html <br>
<br>

UTS3는 다양한 기능을 지원하고 각 기능들의 계산 결과를 마지막에 선별하는 Shader 특성 때문에 사용되지 않더라도 모든 계산을 진행합니다.<br>
UTS3를 사용하여 아트워크를 구성한 후 사용된 기능만 추출해서 정리해도 최적화가 가능합니다.<br>
그리고 Shader Graph를 사용하면 코드 수정 없이 기능을 추가할 수 있으며 협업 시 커뮤니케이션에도 도움이 됩니다.<br>
<br>

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/shadergraph.png?raw=true"/>

작업한 Shader Graph의 경로는 'Assets/UTS3_opt/Shader/UTS3_opt.shadergraph' 입니다.<br>
사용한 기능들만 선택하여 재구성한 Shader Graph 버전의 명칭을 'OPT'(Optimal)라고 명시하겠습니다.<br>
<br>
임의로 캐릭터를 꾸며보고, 여기에 사용된 기능들과 환경을 구체적으로 정리하여 최대한 Shader Graph로 옮겼습니다.<br>
일부 기능은 Shader Graph에서 구현이 불가능하기 때문에, hlsl로 작성되었습니다. (그림자 감쇠값, 광원 색상)<br>

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


두 Shader의 결과물을 동일한지 비교한 결과입니다.<br>
(좌 UTS3 / 우 OPT / 상 그림자 OFF / 하 그림자 ON)<br>

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/comp_no_shadow.gif?raw=true"/>
<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/comp_shadow.gif?raw=true"/>

## 성능 비교

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/speed_test.png?raw=true"/>

'Assets/Scenes/Scene_UTS3_Massive.unity'와 'Assets/Scenes/Scene_Opt_Massive.unity'에서 다른 Shader를 사용한 두 Scene을 구성한 뒤 Rendering Debugger로 실행 속도와 Frame Debugger로 SRP Batch 구조를 비교합니다.<br>
DrawOpaqueObjects의 SRP Batch가 17회로 완전히 동일함을 확인 후, Shader가 병목 지점이 될 수 있도록 4K UHD 해상도로 설정하고 11x11개의 캐릭터 렌더링에 대한 프로파일링 테스트를 진행했습니다.<br>

[링크 - 웹에서 테스트 배치 보기](https://haiun.github.io/UnityChan_TEST/ "WebGl버전 실행")<br>

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/srp_batch_17.gif?raw=true"/>

### 실행 속도

Rendering Debugger / Display Stats에서 DrawOpaqueObjects CPU/GPU 항목에서 유의미한 수치 변화가 있었습니다.<br>

| Case | 그림자OFF CPU | 그림자OFF GPU | 그림자ON CPU | 그림자ON GPU |
| ------ | ------ | ------| ------ | ------ |
| UTS3 | 2.02~2.10 ms | 6.51~6.60 ms | 2.02~2.10 ms | 6.54~6.60 ms |
| OPT | 1.39~1.42 ms | 2.81~2.90 ms | 1.48~1.50 ms | 4.59~4.70 ms |
| 속도향상 | +46.6% | +132.5% | +38.3% | +44.3% |

Frame Debugger으로 확인한 상수 버퍼의 float 갯수가 4624개(float 320개, vector4 4192개, Matrix4x4 112개)에서 188개로 크게 줄었습니다.<br>
Forward+에서 사용하기 위한 최대 256개의 AdditionalLight 관련 값들이 배열로 등록되어 있어 가장 많았고 이외에는 URP의 최대 64개의 ReflProbes 관련 배열이 크기가 컸습니다.<br>

## 결과

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-005.png?raw=true"/>

완전히 동일한 이미지를 약 40% 이상의 속도 향상과 함께, 입력해야 하는 항목이 간소화되어 관리가 쉬워졌습니다.<br>
Shader Graph화를 통해 확장이 용이해졌습니다.<br>

UTS3과 Shader Property를 동일하게 사용하기 때문에 기존 UTS3 Shader에서 사용되던 Material Property를 복사하여 OPS로 쉽게 이식할 수 있습니다.<br>

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-008.png?raw=true"/>

부가적으로 메인 조명 1개만 사용하여 플랫폼 제한이 확장되어 Firefox에서만 실행되던 WebGL 빌드가 크롬과 엣지 브라우저에서도 실행됩니다.<br>


## 외각선 보완




NPR에서 외각선 사용 여부에 따라 화면의 느낌이 크게 달라집니다.<br>

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-015.png?raw=true"/>
<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-012.png?raw=true"/>
<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-017.png?raw=true"/>
<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-013.png?raw=true"/>

하지만 UTS3에서 외각선이 Multi Pass Rendering으로 구현되어 있기 때문에 렌더링 순서가 강제되며, 각각의 Shader Keyword가 달라 비슷한 Material을 연속해서 그리는 것이 중요한 SRP Batch 최적화가 무력화되어, 횟수가 4115번으로 늘어나면서 속도가 심각하게 느려지는 현상을 확인했습니다.<br>

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/srp_batch_failed_outline.gif?raw=true"/>

| Case | 그림자OFF CPU | 그림자OFF GPU | 그림자ON CPU | 그림자ON GPU |
| ------ | ------ | ------| ------ | ------ |
| UTS3 중앙값 | 2.06 ms | 6.55 ms | 2.06 ms | 6.57 ms |
| UTS3+외각선 | 73 ms | 37 ms | 74 ms | 40 ms |

Universal Renderer Data에서 Render Objects나 MaterialPropertyBlock을 사용하는 방법이 적합하지 않아, 간단한 단색 외각선 Shader Graph를 작성하여 런타임에 외각선 메쉬를 직접 생성하는 추가 테스트를 진행했습니다.<br>
이후, 외각선용 Shader Graph의 이름을 'MeshBackfaceOutline'으로 명시하며, 내용은 아래와 같습니다.<br>

### Shader Graph

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/K-006.png?raw=true"/>

### 렌더링

(좌 UTS3 / 우 OPT / 상 그림자 OFF / 하 그림자 ON)<br>
<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/simple_outline.gif?raw=true"/>
<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/simple_outline_shadow.gif?raw=true"/>

### 성능 측정

<img src="https://github.com/haiun/URP_UTS3ShaderGraph/blob/main/ReadmeImage/srp_batch_outline.gif?raw=true"/>

Multi Pass Rendering이 아닌 일반적인 다른 오브젝트로 처리되면서 SRP Batch가 26회 진행됨을 확인했으며, Skinning 연산이 2배 증가했습니다.<br>
물리적으로 오브젝트가 약 2배가량 늘었기 때문에, 모든 수행 시간이 어느 정도 비례하여 증가할 것으로 추측했습니다.<br>
테스트 결과는 예상과 비슷했습니다.<br>

DrawOpaqueObjects CPU/GPU 항목에서 측정한 시간은 아래와 같습니다.

| Case | 그림자OFF CPU | 그림자OFF GPU | 그림자ON CPU | 그림자ON GPU |
| ------ | ------ | ------| ------ | ------ |
| UTS3+MeshBackfaceOutline | 4.31~4.51 ms | 5.88~5.91 ms | 4.26~4.63 ms | 5.82~5.89 ms |
| OPT+MeshBackfaceOutline | 3.42~3.60ms | 3.12~3.35 ms | 3.53~3.73 ms | 3.18~3.31 ms |

수치 비교는 DrawOpaqueObjects 항목만 진행했지만, 그림자 맵을 포함한 다른 여러 부분에서 CPU 부하가 비례하여 약 2배 증가했습니다.<br>
하지만 UTS3의 외각선을 사용한 속도보다 훨씬 더 가벼웠습니다.<br>
UTS3의 효과 품질에 비해 외각선은 기본에 충실한 표현이지만, 비교적 10배 이상의 성능 향상을 확인했습니다.<br>
