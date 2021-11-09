using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public enum CharacterName { Fox, Hunter };
    public enum CharacterPosition { Center, Left, Right };
    public enum CharacterMood { Idle, Idle_2, BitHappy, Happy, Uncomfortable, Surprised, Sad, Upset };
}

/*
1. 기본 : Idle
2. 턱을 괸 기본 표정 : Idle_2
3. 살짝 웃음 : BitHappy
4. 크게 웃음 : Happy
5. 시선을 아래로 내린 어색한 표정 : Uncomfortable
6. 당황한 or 놀란 표정 : Surprised
7. 슬픈 표정 : Sad
8. 화난 표정 : Upset
*/
