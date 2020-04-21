using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum EmTagName {
    Bullet = 1,
}


//世界物件类型
public enum EmObjType
{
    E_Brick = 0, //城墙-砖
    E_Grass = 1, //草地
    E_Iron = 2,  //城墙-铁
    E_River = 3, //河流
    E_Boss = 4,  //BOSS
}

//敌军类型 10、普通 11、普通红色 20、快速 21、快速红色 30、高级白色  31、高级红色 32、高级黄色 33、高级绿色
public enum EmEnemyType
{
    E_Simple = 10,
    E_SimpleRed = 11,
    E_Fast = 20,
    E_FastRed = 21,
    E_Advance = 30,
    E_AdvanceRed = 31,
    E_AdvanceYellow = 32,
    E_AdvanceGreen = 33,
}
