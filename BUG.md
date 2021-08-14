﻿# 问题汇总

玩家提供的建议、发现的bug等。

# 进行中

* PC选项：选项无法使用键盘完成。

* 视频背景不能显示的问题。

  似乎仅在Windows 7无效。

* 手柄：加大，并且不要离屏幕边缘太近。

* 病房可能因为碰撞箱问题很难从右侧与雪兰对话

  可能是新的网格化模式适配存在问题。

* 地图移动模式建议

  寻路修改为只对可调查/对话的物件有效。

  调整人物的碰撞箱大小。

  >  寻路无法移动到超出视野的位置，而轮盘要与NPC面对面操作不便。

* 测试存档点

  拉动轮盘调查床时，轮盘不会消失，并且将陷入调查死循环，床的调查脚本被死循环触发。

  松开轮盘无法解决问题，尚不清楚是否和**触摸判定**有联系。

* 门卡死问题（尚未确定问题）

  从雪兰病房出去后进入兮的病房的门，回到雪兰病房门口后，再进去病房会导致卡死。

* 界面布局调整

  部分按钮容易误触，需要调整。

* 轮盘与寻路的配合移动模式问题

  手机端似乎可以通过多点触控同时触发二者，导致移动异常（极速移动）。

* 寻路功能建议

  当前寻路未完成时，仍然可以触发下一次寻路，并覆盖上一次寻路。

* 移动端的触摸判定不灵敏，存在很多问题。

  由该问题造成的bug如下：

  1. 轮盘有时不会被呼出，而是呼出了寻路模式。
  2. 使用摇柄控制方向的时候，点击调查时摇柄不松手的话会一直卡在调查界面，摇柄松手后才能继续

# 未确认的问题

* 手柄：手机无法控制。

# 不会被处理的问题

* 雨：直线下的时候不应该有雾气。

  雨天组件为第三方资源，不能随意改动。

# 已处理

* 寻路：好像有的地方无法移动。

  网格化不精确导致。

* 床：玩家应该能够躺上去。

  已经添加对应的剧本脚本。

* 不要让玩家看到门被隐藏了。

* 半透明墙：在门的周围加上，比较真实。

  还需要在视觉效果上做调整。

* 自由移动的显示问题。

  由于移动只允许四个方向，因此视觉效果不应包含360度的移动方向。