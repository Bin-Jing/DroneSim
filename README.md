
# DroneSim

# 安裝流程：
- git clone https://github.com/Bin-Jing/DroneSim.git
- Download unity [tensorflowsharp](https://s3.amazonaws.com/unity-ml-agents/0.5/TFSharpPlugin.unitypackage)
- Download [ml-agent 5.0a](https://github.com/Unity-Technologies/ml-agents/releases/tag/0.5.0a)

- Open unity and open DroneSim project
- Open and install tensorflow sharp
- Copy ml-agents5.0a/UnitySDK/Assets/ML-Agents to DroneSim project Assets

註：7.0版本已經包含了 tensorflowsharp 所以可能以後會載不到
建議升級版本
# DroneMove
![](https://i.imgur.com/SoUKe6f.png)

這個Script是用來控制無人機物理和如何飛行的
constValue是用來設定一些定值計算物理時使用的

但現在AirSim已經有Unity版本，他的物理模式比我的好，建議可以直接改成使用AirSim

# ML-Agent使用方式
![](https://i.imgur.com/17fGPqy.png)

接著是ML-Agent使用方式，我所使用的版本為ml-agent5.0版本我看到6.0版本以後使用方式有些不同但我這個還沒改，5.0以後版本詳細看https://github.com/Unity-Technologies/ml-agents/tree/master/docs 教學

## 以下為5.0a版本說明
建議先看完上面連結的官方教學再來看我寫的內容比較清楚。
ML-Agent使與時需要三個重要的Script為

Agent、Brain、Academy

- Agent是用來設定RL在訓練時需要觀測到哪些值和RL最後要學到什麼，Agent需要的參數為如上圖，

- Brain這個Script和是否需要用Camera來讓訓練的Agent有影像的觀測，可藉由Add和remove Camera兩個按鈕來新增或減少Camera數量，MaxStep為這個Agent執行多少步行為後他會強制讓這個Agent結束這一輪，設定為0時為不強制重製，Reset On Done指的是當Done時是否要去呼叫AgentReset這個funciton，
- On Demand Decision是指是否要根據你的命令來影響RL的決策，Decision Frequency是每做幾個Step更新一次RL的決策一定要大於1建議在10以下不然不能收斂。剩下的參數為我定義的，主要用來隨機生成物件。
- 
我在DroneAgent2這個Script裡面繼承了Agent然後寫了每次重製時無人機的生成、障礙物隨機生成和目標點的隨機生成，Reward的計算也都寫在裡面
裡面最主要的function為AgentReset 、CollectObservations 、AgentAction 
這三個function是我們需要去override ，
AgentReset是當呼叫到unity給的Done function時他會執行的function也就是每一輪結束時會呼叫的，例如撞到障礙物或到達終點時我會呼叫Done這個function這時他就會呼叫AgentReset，所以可以在AgentReset這個function裡面寫整個環境的初始化。
CollectObservations是要寫有關RL需要觀測到的Data，所觀測到的Data需要做normalize這樣RL比較好學到東西。
AgentAction是要寫如何操控這個Agent，我在裡面設定了無人機怎麼動和他動了後得到的reward。


# Academy
![](https://i.imgur.com/2WssQSf.png)

本版未以此方法寫curriculum的方式learning 但可以使用這個方法來隨著訓練結果的成長，來增加訓練難度

- Academy主要設定某些參數在訓練時是否需要修改，如使用curriculum learning時到達某個條件時增加或減低難度
- MaxStep為每個Agent執行到幾部時強制重製，設定為0時為不強制重製，Training Configuration為訓練時的視窗設定
- Inference configuration為跑訓練的model時的視窗設定
- Reset Parameter為設定某些參數在訓練時是否需要修改


# Brain
![](https://i.imgur.com/xC3VnRi.png)


- Brain為傳給RL的一些參數設定，Vector Observation為Agent裡面CollectObservations這function你所寫的有幾個值要傳給RL當訓練資料的，Stacked Vector為有幾個stack

- Visual Observation為你在Agent裡面設定了幾個Camera，Width和Height為畫面的長寬Black And White為是否要改為使用黑白圖片
- Vector Action可設定為Continuous或discrete
- Space size為6就是上下左右前後
- Brain Type為Internal時是使用訓練完成的model 將訓練完成的model放到Graph model裡面，Brain Type為External時是要訓練時，



- Brain Type為Player時為設定無人機怎麼動的，和Agent的AgentAction有關，我這裡設定為Continuous且Size為4，index為3的是左右移動index為1的是前後移動，index為0的是旋轉，index為2的是上下移動


因此AgentAction裡面這樣寫:

`
rotataionY = vectorAction[0];
directionY = Mathf.Clamp(vectorAction[2], -1f, 1f);
directionZ = Mathf.Clamp(vectorAction[1], -1f, 1f);
directionX = Mathf.Clamp(vectorAction[3], -1f, 1f); 
`

然後就可以將rotataionY 、directionY 、directionZ 、directionX 作為參數帶入DroneMove這個Script讓無人機移動了。
將來若要改AirSim也是將這些移動帶到AirSim的移動function裡面。


# 訓練時輸入：


mlagents-learn <trainer-config-file> --env=<env_name> --run-id=<run-identifier> --train

- <trainer-config-file>為training_config檔案放置位置
- --env= unity build出來的執行檔位置
- --run-id 為這次訓練的名稱
- --train 為訓練
若之前有訓練了就使用—load來load之前訓練的模型，--run-id要和之前訓練的模型相同

註：版本必須為5.0a才能跑，不然就需要升級這個project ml-agents版本

# Training Config 參數
~~~
    trainer: ppo
    batch_size: 1024 
    beta: 1.0e-2
    buffer_size: 10240 
    epsilon: 0.2
    gamma: 0.99
    hidden_units: 128 
    lambd: 0.99 
    learning_rate: 3.0e-4
    max_steps: 300.0e5
    memory_size: 256
    normalize: true #false
    num_epoch: 3
    num_layers: 2 
    time_horizon: 64
    sequence_length: 32
    summary_freq: 10000
    use_recurrent: true
    use_curiosity: false
    curiosity_strength: 0.01
    curiosity_enc_size: 256 
~~~