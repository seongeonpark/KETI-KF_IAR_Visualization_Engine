# KETI-KF_IAR_Visualization_Engine
산업용 증강현실 가시화 엔진

## 목차
1. [Github Configuration](#part1)
2. [Environment](#part2)
3. [Project Set up](#part3)
* [VR/AR Settings](#part3-1)
* [Vuforia Configutation](#part3-2)
4. [Prefab 설정](#part4)

---

<a id="part1"></a>
## Github Configuration  
Version Contrl Mode를 Visible Meta Files로 설정한다.  
![image](https://user-images.githubusercontent.com/21256263/102186063-69ed4880-3ef5-11eb-9b05-8855e73d8d8c.png)  

---

<a id="part2"></a>
## Environment  
Unity3D: 2019.1.14f  
Vuforia Engine: 8.3.8  
<img src="https://user-images.githubusercontent.com/21256263/102186309-c6e8fe80-3ef5-11eb-8926-6ffb031bbd8a.png" height="400">  

---

<a id="part3"></a>
## Project Set up
<a id="part3-1"></a>
### 1. Unity Settings
#### Build Settings
- Switch platform to Android
<img src="https://user-images.githubusercontent.com/21256263/102295535-e1bb8180-3f8e-11eb-82f3-5d9ccad86970.png" width="500">

#### VR/AR Settings
- XR Settings  
Check Virtual Reality Supported
<img src="https://user-images.githubusercontent.com/21256263/102298971-79bc6980-3f95-11eb-8cd3-64cb57db6938.png" width="500">

- Other Settings
<img src="https://user-images.githubusercontent.com/21256263/102298981-7d4ff080-3f95-11eb-9d7d-aecb1f3bfb7d.png" width="500">

<a id="part3-2"></a>
### 2. Vuforia Configutation

1. Unity 에디터의 메뉴바에서 GameObject > Vuforia Engine > AR Camera를 추가한다.  
(게임 씬에서 제공하는 기본 main camera 오브젝트는 필요없으니 삭제해도 무방하다)
<img src="https://user-images.githubusercontent.com/21256263/102307465-de80bf80-3fa7-11eb-87e1-2e2f3818f4e3.png" width="500"/>

2. 추가된 AR Camera의 Inspector 설정은 아래와 같다.
<img src="https://user-images.githubusercontent.com/21256263/102307096-2c48f800-3fa7-11eb-8eed-8d91c9d283dc.png" width="500"/>

3. Vuforia Configuration은 아래와 같다.
<div>
<img src="https://user-images.githubusercontent.com/21256263/102307638-3f0ffc80-3fa8-11eb-8a73-8afd27d2967c.png" width="200"/>
<img src="https://user-images.githubusercontent.com/21256263/102307740-72528b80-3fa8-11eb-9318-7457eef4874d.png" width="200"/>
<img src="https://user-images.githubusercontent.com/21256263/102307647-40d9c000-3fa8-11eb-83d5-dc68fb4d6dc0.png" width="200"/>
<img src="https://user-images.githubusercontent.com/21256263/102307653-42a38380-3fa8-11eb-84c1-9ecf7e311afb.png" width="200"/>
<div/>  

---
 
<a id="part4"></a>
## Prefab 설정
1. 타겟 오브젝트 설정하기  
1-1. 오브젝트를 생성하고, 아래와 같이 3개의 스크립트를 추가한다.  
1-2. 추가된 스크립트 중 Image Target Behaviour에서 타겟 이미지를 추가한다. (-> Add Target)  
1-3. 뷰포리아 사이트에 등록한 이미지를 다운(유니티 패키지 형식)받아 해당 프로젝트에 임포트한다.  
<div>
  <img src="https://user-images.githubusercontent.com/21256263/102843531-b8966780-444c-11eb-96e2-c3e0d8fb3c42.png" height="500"/>
  <img src="https://user-images.githubusercontent.com/21256263/102843908-9b15cd80-444d-11eb-9a5f-d7ba94128bba.png" height="500"/>
  <img src="https://user-images.githubusercontent.com/21256263/102843251-2c844000-444c-11eb-82df-ba9872c50df4.png" width="800"/>
</div>
