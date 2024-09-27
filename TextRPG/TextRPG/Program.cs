using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Text.Json;

namespace TextRPG
{
    //구성
    // 0. 데이터 초기화
    // 1. 스타팅 로고를 보여줌 (게임 처음 킬 때만)
    // 2. 선택 화면을 보여줌 (기본 구현사항 - 상태 / 인벤토리)
    // 3. 상태 화면을 구현함 ( 필요 구현 요소 : 캐릭터, 아이템)
    // 4. 인벤토리 화면 구현함
    // 4-1. 장비장착 화면 구현
    // 5. 선택화면 확장


    //  P.s ->  숫자. 은 코드 작업 순서를 표시 한 것.



    // 3. 플레이어를 생성하기 위해 클래스가 필요함, 캐릭터와 아이템 클래스를 구현할 예정
    public class Character // 3-1. 캐릭터 클래스 정의, 캐릭터의 구성도
    {
        public int gameData {  get; set; }
        public string Name { get; set; } // 한 번 정의되면 바꿀 수 없도록 함
        public string Job { get; set;  } // Enum은 아직 배우지 않았으므로 pass
        public int Level { get; set; }
        public int Atk { get; set; }
        public int Def { get; set; }
        public int Hp { get; set; } // 사냥이나 휴식하기 등의 기능 때문에 set; 추가
        public int Gold { get; set; } // 아이템 구매 판매 등 유동적이므로 set; 추가

        public Character(string name, string job, int level, int atk, int def, int hp, int gold)
        // 3-2. 캐릭터 클래스의 생성자, 기본적으로 클래스의 이름과 같은 함수, 캐릭터를 실제로 생성하는 과정 = 인스턴스를 만든다
        {
            Name = name;
            Job = job;
            Level = level;
            Atk = atk;
            Def = def;
            Hp = hp;
            Gold = gold;
            // 조금 이따가 GameDataSetting()에 new 캐릭터를 생성 할 때, (인스턴스를 만들 때)
            // 생성자에서 설정한 기본 세팅 조건들이 new player에게 자동으로 대입 됨.
        }
    }

    public class Item // 4. 아이템 클래스 정의
    {
        public string Name { get; set; }
        // get 접근자 : 속성 값을 읽는 데 사용, 클래스의 외부에서 속성의 값을 요청할 때 get 접근자의 코드가 실행
        // set 접근자 : 속성 값을 할당 하는데 사용, 클래스의 외부에서 속성에 특정 값을 할당. (골드, 체력, 공격력 방어력 등 변하는 값에 사용 할 예정)
        // get; 만 있다 = 읽기 전용, 객체가 생성될 때 설정 되고 이후에는 변경 할 수 없음. 
        // 둘 다 있다   = 읽기/쓰기 전용, 클래스 외부에서 값을 변경할 수 있음, 속성이 동적으로 변경될 필요가 있는 경우 사용.
        // ∴ 플레이어의 레벨이나 체력이 게임 도중 변경될 수 있다면(사냥을 한다거나) set 접근자가 필요할 수 있다.


        public string Description { get; set; } // 템 설명

        public int Type { get; set; } // 무기 or 방어구 타입

        public int Atk { get; set; }

        public int Def { get; set;  }

        public int Gold { get; set; } // set을 추가 하지않아(읽기 전용이었어서) 골드가 변하지 않는 문제가 있었음, 그래서 추가

        public bool IsEquipped { get; set; } // 4-1. 장착 유무, 장착이 실제로 되었는지 확인하기 위함

        public bool IsPurchased { get; set; } // 8. 상점 관련, 구매 여부를 나타내는 속성

        public static int ItemCnt = 0;
        // 5-2.  클래스에 공유가 되는 int형 변수
        // 각각의 인스턴스가 아닌, 아이템 클래스에 귀속 되어 게임에 전반적으로 공유 되는 변수, 아이템 숫자 관련 함수의 for 조건문 등에 활용 될 예정
        // 아이템이 만들어 질 때마다 변수를 1만큼 올리고 싶을 때, 일일히 인스턴스에서 만들기 보단 전체에 공유 되는 값을 위함.

        public Item(string name, string description, int type, int atk, int def, int gold, bool isEquipped = false)
        // 4-2. 아이템 클래스의 생성자, 장착 유무는 false로 설정(처음에 안 끼고 있으니)
        {
            Name = name;
            Description = description;
            Type = type;
            Atk = atk;
            Def = def;
            Gold = gold; // 이 부분 작성을 빼먹어 골드값이 0으로 나오는 문제가 있었음.
            IsEquipped = isEquipped;
            IsPurchased = false; // 8. 상점 관련, 기본적으로 아이템은 구매되지 않은 상태
        }

        public void PrintItemStatDescription(bool withNumber = false, int idx = 0, bool showPrice = true, bool inInventory = false)
        // 1. 매개변수에 bool inInventory = false를 추가 
        {
            Console.Write("- ");

            if (withNumber)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write("{0} ", idx);
                Console.ResetColor();
            }


            // 2. 요 if 문 매개 변수 안에 "&& inInventory"를 추가
            if (IsEquipped && inInventory) // 둘 다 true면,(장착했다면 + inInventoy가 ture면) [E] 표시   (인벤토리에서만 보이게 할 예정) 
            {
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("E");
                Console.ResetColor();
                Console.Write("]");
                Console.Write(PadRightForMixedText(Name, 9));
            }
            else
            {
                Console.Write(PadRightForMixedText(Name, 12));
            }

            Console.Write(" | ");

            // 수치가 0이 아니라면 작동,  [ 삼항 연산자 활용 => 조건 ? 조건이 참이라면 : 조것이 거짓이라면 ]
            if (Atk != 0) Console.Write($"Atk {(Atk >= 0 ? " + " : "")}{Atk}");  // 공격력이 0보다 크거나 같으면 "+" 를 붙이고 아니면 " "해라(붙이지 마라).
            if (Def != 0) Console.Write($"Def {(Def >= 0 ? " + " : "")}{Def}");

            Console.Write(" | " + Description); // 설명 출력
            if (showPrice) // 8. 상점관련, 가격 및 구매 완료 표시 bool 코드
            {
                // IsPurchased가 true이면 "구매완료", 아니면 가격을 출력
                string priceOrStatus = IsPurchased ? "구매완료" : Gold + " G";
                Console.WriteLine(" | " + priceOrStatus);
            }
            Console.WriteLine();
        }

        public static int GetPrintableLength(string str) // 아이템 텍스트 정렬을 위해 Length의 길이를 구함
        {
            int length = 0;
            foreach (char c in str)
            {
                if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherLetter)
                {
                    length += 2; // 한글과 같은 넓은 문자는 길이를 2로 취급
                }
                else
                {
                    length += 1; // 나머지 문자는 길이를 1로 취급
                }
            }
            return length;
        }
        public static string PadRightForMixedText(string str, int totalLength)
        {
            int currentLength = GetPrintableLength(str); // 텍스트의 실제 길이
            int padding = totalLength - currentLength; //  총길이 - 실제길이 = int padding 추가해야 할 길이
            return str.PadRight(str.Length + padding); // padding 만큼 PadRight(문자열의 오른쪽)에 공백을 추가
        }

    }
    internal class Program
    {
        static string FilePath = Directory.GetCurrentDirectory();
        static Character _player; // 5. 실제 플레이에서 쓸 캐릭터와 아이템 추가,  class Program 내에서 플레이어 관련에 주구장창 사용 예정
        static Item[] _items;     // 아이템은 여러 개 이므로 배열[] 사용, class Program내에서 아이템 관련 사용 예정
        

        static void Main(string[] args) // 메인 함수
        {

            GameDataSetting(); // 1. 게임 데이터 세팅
                               // new 라는 키워드를 통해 새로운 캐릭터를 메모리에 할당 지시, Character 생성자 발동

            PrintStartLogo(); // 5-4. 게임 스타트 화면 함수

            StartMenu(); // 5-5. 스타트 메뉴
        }
        private static void GameDataSetting() // 2. 게임 데이터 메서드 생성, Private(비공개)
        {
            _player = new Character("스파르타", "전사", 1, 10, 5, 100, 1500); // 5-1. new 플레이어 변수 선언(실제로 사용할 캐릭터의 데이터)
            // 아까 3-2 에서 만든 캐릭터 클래스 생성자의 세팅 값들을 받아 옴.
            // () 괄호 안에 각각 순서대로 Name, Job, Level, Atk, Def, HP, Gold 순서이며, 입력 값이 세팅 됨.
            // _ (언더스코어) 사용 이유? = Private(비공개) 필드 구분을 위한 것.
            // 클래스 내부에서만 사용되는 변수임을 쉽게 식별할 수 있고, 외부에서 접근되는 공개 속성이나 메서드와의 혼동을 방지.
            // 복습 : 클래스의 필드(field)란 클래스에 포함된 변수(Variable)를 말한다. ( 결국 같은 뜻? )
            // 변수에는 특정 값을 할당할 수 있고, 이를 통해 객체의 특성을 만들어줄 수 있다.

            _items = new Item[9]; // 이번에는 List 대신 배열을 사용, 추후 5-3. additem 메서드를 정의 할 예정, (위의 5-2 로 이동)

            // 5-4. 아이템 추가
            AddItem(new Item("가죽 갑옷", "가죽으로 만든 가볍고 약한 갑옷 이다.", 0, 0, 5, 1000)); // 맨 앞의 숫자가 0이면 방어구, 1이면 무기
            AddItem(new Item("사슬 갑옷", "사슬을 엮어 만든 조금은 방어가 수월한 갑옷이다.", 0, 0, 9, 2000));
            AddItem(new Item("스파르타의 갑옷", "스파르타의 전사들이 사용하는 강력한 갑옷이다.", 0, 0, 15, 3500));
            AddItem(new Item("성스러운 갑옷", "천국의 축복을 받은 성스러운 힘이 착용자를 지켜주는 방어구 이다.", 0, 0, 30, 5500)); // 추가 아이템 1
            AddItem(new Item("낡은 십자가 검", "낡은 십자가 안에 숨겨진 무딘 검 이다.", 1, 2, 0, 600));
            AddItem(new Item("무쇠 도끼", "무쇠로 만들어진 날카로운 도끼 이다.", 1, 5, 0, 1500));
            AddItem(new Item("스파르타의 창", "스파르타의 전사들이 사용하는 무엇이든 찌르는 창 이다.", 1, 7, 0, 3000));
            AddItem(new Item("축복받은 검", "사제들이 축복을 내린 검, 악마들을 베는데 효과적이다.", 1, 13, 0, 4500)); // 추가 아이템 2
            AddItem(new Item("롱기누스의 창", "성스러운 성창, 그 어떤 불경스런 존재도 이것을 견딜수는 없다", 1, 99, 0, 9999)); // 추가 아이템 3
        }

        static void StartMenu() // 5-5. 스타트 메뉴
        {
            Console.Clear(); // 게임 스타트 화면 정리
            Console.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            Console.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
            Console.WriteLine();  // Console.WriteLine(); 단축키 -> C + W + Tab
            Console.WriteLine("");
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전 입장");
            Console.WriteLine("5. 휴식");
            Console.WriteLine("6. 게임 저장 및 불러오기");
            Console.WriteLine("7. 게임 종료");
            Console.WriteLine("");


            // int keyInput = int.TryParse(Console.ReadLine(), out keyInput);  는 안 쓰고 아래 따로 함수 생성
            // CheckValidInput(1, 7);   // 5-6.  1에서 7까지 유효성 확인, 일단 관련 함수들을 만들고 switch 문의 매개변수로 사용될 예정

            switch (CheckValidInput(1, 7))
            // 5-8. switch문, CheckValidInput함수로 유효 값(1~7)을 입력 받으면 그에 맞는 함수 호출 후 break;로 벗어남.

            {
                case 1:
                    StatusMenu(); // 플레이어 상태
                    break;
                case 2:
                    InventoryMenu(); // 인벤토리
                    break;
                case 3:
                    StoreMenu(); // 상점
                    break;
                case 4:
                    DungeonMenu(); // 던전 입장
                    break;
                case 5:
                    RestMenu(); // 휴식
                    break;
                case 6:
                    SaveGameMenu(); // 게임 저장 및 불러오기
                    break;
                case 7:
                    GameOverMenu(); // 게임 종료
                    break;
            }
        }

        private static void StatusMenu() // 6. 플레이어 상태
        {
            // 6-3. 상태창 꾸미기 작업 시작
            Console.Clear();
            ShowHighlightText("■ 상태 보기 ■"); // 제목에 첫 줄 색 변경 6-2 함수 활용
            Console.WriteLine("캐릭터의 정보가 표기됩니다.");

            PrintTextWithHighlights("Lv ", _player.Level.ToString("00")); // 문자열 하이라이트 6-2 함수 활용, "00" 은 01,02,03 이런 식으로 두 자릿수로 표현
            Console.WriteLine("");
            Console.WriteLine("{0} ({1})", _player.Name, _player.Job);

            // 7-8. 합산 공격력, 방어력 구현 추가
            int bonusAtk = getSumBonusAtk(); // 밑에 넣어줄 예정
            int bonusDef = getSumBonusDef();
            PrintTextWithHighlights("공격력 : ", (_player.Atk + bonusAtk).ToString(), bonusAtk > 0 ? string.Format(" (+{0})", bonusAtk) : "");
            // 플레이어 공격력 + 보너스 공격력을 문자열로, (삼항연산자) 보너스 공격력이 0보다 크면 +(보너스 어택)을 출력해주고, 아니면은 빈칸을 추가
            PrintTextWithHighlights("방어력 : ", (_player.Def + bonusDef).ToString(), bonusDef > 0 ? string.Format(" (+{0})", bonusDef) : "");

            // 각각 PrintTextWithHighlights 함수의 s1 , s2, s3인데 Atk(공격력) 의 자료형은 Int이므로 s2의 노란색 컬러를 적용 시키기 위해 Tostring 해줌
            PrintTextWithHighlights("체력 : ", _player.Hp.ToString());
            PrintTextWithHighlights("골드 : ", _player.Gold.ToString());
            Console.WriteLine("");
            Console.WriteLine("0. 뒤로가기");
            Console.WriteLine("");

            switch (CheckValidInput(0, 0)) // 0번 나가기
            {
                case 0:
                    StartMenu();
                    break;
            }
        }

        private static int getSumBonusAtk() // 7-7. 공격력 합산 표시
        {
            int sum = 0; // 능력치를 다 더 할 것 
            for (int i = 0; i < Item.ItemCnt; i++)   // 아이템을 전부 확인
            {
                if (_items[i].IsEquipped) sum += _items[i].Atk;
                // 아이템 목록의 아이템이 장착되어 있다면, 아이템의 Atk를 다 더해라.
            }
            return sum; // 그 다음에 리턴해라.
        }
        private static int getSumBonusDef()  // 방어력 합산
        {
            int sum = 0;
            for (int i = 0; i < Item.ItemCnt; i++)   // 아이템을 전부 확인
            {
                if (_items[i].IsEquipped) sum += _items[i].Def;
            }
            return sum;
        }


        private static void InventoryMenu() // 7. 인벤토리 
        {
            Console.Clear();
            ShowHighlightText("■ 인벤토리 ■");
            Console.WriteLine("보유중인 아이템을 관리 할 수 있습니다.");
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");
            Console.WriteLine("");

            // 위쪽 Item 클래스로 이동해서 작업 7-1로.

            // 7-2 아이템 설명 출력 반복문
            for (int i = 0; i < Item.ItemCnt; i++) // 아이템의 Itemcnt 반복문, 아이템의 가짓 수 만큼 출력되어 보임
            {
                _items[i].PrintItemStatDescription(true, i + 1); // true면, _items의 i번째에서 7-1의 PrintItemStatDescription();(아이템 설명) ++ 출력
            }
            Console.WriteLine("");
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 장착관리");
            Console.WriteLine("");

            switch (CheckValidInput(0, 1)) // 0 or 1
            {
                case 0:
                    StartMenu();
                    break;
                case 1:
                    EquipMenu(); // 장착 관리
                    break;
            }
        }

        private static void EquipMenu() // 7-3. 장착관리 메뉴
        {
            Console.Clear();

            ShowHighlightText("■ 인벤토리 - 장착 관리 ■");
            Console.WriteLine("보유중인 아이템을 장착/해제 할 수 있습니다.");
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");
            Console.WriteLine("");
            // 잠시 PrintItemStatDescription함수로 이동, withNumber 변수를 활용 예정
            for (int i = 0; i < Item.ItemCnt; i++)
            {
                _items[i].PrintItemStatDescription(true, i + 1);
            }

            Console.WriteLine("");
            Console.WriteLine("0. 나가기");

            // 7-5.  default를 활용한 switch문 ->  모든 케이스가 아니면, 마지막에 케이스 default가 실행
            int keyInput = CheckValidInput(0, Item.ItemCnt); // 아까 만든 입력 유효성 확인 함수(0에서 아이템 숫자만큼) 인풋값에 활용
            switch (keyInput)
            {
                case 0:
                    InventoryMenu();
                    break;
                default:
                    ToggleEquipStatus(keyInput - 1); // ToggleEquipStatus는 곧 만들고(아이템 장착 상태 변경),
                                                     // 유저 입력값은 123이며 실제 배열에는 012 이므로 -1해서 맞춰줌
                    EquipMenu();
                    break;
            }
        }
        private static void ToggleEquipStatus(int idx) // 7-6. 아이템 장착 상태 변경  
        {
            // 타입 별로 하나의 아이템만 장착
            // 같은 타입의 다른 아이템이 이미 장착되어 있다면 해제
            for (int i = 0; i < Item.ItemCnt; i++)
            {
                if (i != idx && _items[i].IsEquipped && _items[i].Type == _items[idx].Type)
                {
                    _items[i].IsEquipped = false; // 기존 아이템 해제   // IsEquipped;가 true면 [E]가 나온다.
                }
            }
            // 새 아이템 장착 / 해제
            _items[idx].IsEquipped = !_items[idx].IsEquipped; // _items의 목록[idx]에 들어가서 IsEquipped이면, !(bool값을 반대로)로 장착 상태 변경

        }


        private static void StoreMenu() // 8. 상점
        {
            Console.Clear();
            ShowHighlightText("■ 상 점 ■");
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine("");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine("");
            PrintTextWithHighlights("", _player.Gold.ToString(), " G"); // 문자열 s2만 노란 색이기 때문에 앞에 "" 로 s1 추가해줬음, 관련 메서드 새로 만드는 것보단 편법 사용. 그래서 골드 값은 s2(노랑)으로 출력
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");
            Console.WriteLine("");

            for (int i = 0; i < Item.ItemCnt; i++)
            {
                _items[i].PrintItemStatDescription(false, i + 1, true); // 아이템 설명 출력 메서드를 사용합니다.
            }
            // withNumber 파라미터를 false로 설정하여 상점에서만 아이템 번호 없이 출력
            // true값 하나 더 추가, 상점에서 아이템을 출력할 때 showPrice(가격 보여주기)를 true로 설정하여 가격을 출력 (기본이 true여서 넣을 필요는 없었던듯)
            // 인벤토리에선, 가격 표시를 사용 안하므로 2개의 매개변수만 가짐. (true, i + 1)


            Console.WriteLine("");
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 아이템 구매");
            Console.WriteLine("2. 아이템 판매");
            Console.WriteLine("");

            switch (CheckValidInput(0, 2))
            {
                case 0:
                    StartMenu();
                    break;
                case 1:
                    BuyItemMenu();
                    break;
                case 2:
                    SellItemMenu();
                    break;
            }
        }

        private static void BuyItemMenu() // 8-1. 구매
        {
            Console.Clear();
            ShowHighlightText("■ 상 점 ■");
            Console.WriteLine("필요한 아이템을 구매 할 수 있습니다.\n");
            Console.WriteLine("\n구매하고 싶은 아이템 번호를 입력 해주세요.");
            Console.WriteLine("0을 입력하면 상점으로 돌아갑니다.");
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");
            Console.WriteLine("");
            for (int i = 0; i < Item.ItemCnt; i++)
            {
                _items[i].PrintItemStatDescription(true, i + 1);
            }

            int choice = CheckValidInput(0, _items.Length); // 0 입력 가능
            if (choice == 0) // 0 입력 시 상점 메뉴로 복귀
            {
                StoreMenu();
                return;
            }

            choice -= 1; // 사용자 입력에서 1을 빼서 실제 인덱스로
            Item selectedItem = _items[choice];
            if (selectedItem.IsPurchased)   // 이미 구매한 아이템인지 확인
            {
                Console.WriteLine("이미 구매한 아이템입니다.");
            }
            else if (_player.Gold >= selectedItem.Gold) // 플레이어 골드가 선택 아이템 골드보다 크면 = 구매 가능한 경우
            {
                selectedItem.IsPurchased = true; // 구매 표시를 true로
                _player.Gold -= selectedItem.Gold; // 플레이어 골드 감소
                Console.WriteLine($"\n{selectedItem.Name} 구매를 완료했습니다.");
            }
            else // 골드가 부족한 경우
            {
                Console.WriteLine("Gold가 부족합니다.");
            }
            Console.WriteLine("아무 키나 누르면, 상점으로 돌아갑니다.");
            Console.ReadKey();
            StoreMenu();
        }

        private static void SellItemMenu() // 8-2. 판매
        {
            Console.Clear();
            ShowHighlightText("■ 상 점 ■");
            Console.WriteLine("필요한 아이템을 판매 할 수 있습니다.\n");
            Console.WriteLine("판매하고 싶은 아이템 번호를 입력 해주세요.");
            Console.WriteLine("\n0을 입력하면 상점으로 돌아갑니다.");
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");
            Console.WriteLine("");
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i].IsPurchased) // 구매한 아이템만 표시
                {
                    _items[i].PrintItemStatDescription(true, i + 1, false);
                    // PrintItemStatDescription 메서드를 호출할 때 showPrice를 false로 설정하여 가격 대신 판매가를

                    int sellPrice = (int)(_items[i].Gold * 0.85); // 실제 판매가를 계산.
                    Console.WriteLine($" - 판매가: {sellPrice} G");
                }
            }

            int choice = CheckValidInput(0, _items.Length); // 0 입력 가능
            if (choice == 0) // 0 입력 시 상점 메뉴로 복귀
            {
                StoreMenu();
                return;
            }

            choice -= 1; // 사용자 입력에서 1을 빼서 실제 인덱스로 변환
            Item selectedItem = _items[choice];
            if (selectedItem.IsPurchased)
            {
                int sellPrice = (int)(selectedItem.Gold * 0.85); // 실제 판매가를 계산
                _player.Gold += sellPrice; // 플레이어의 골드를 판매가만큼 증가
                selectedItem.IsPurchased = false; // 아이템의 구매 상태를 false로 설정
                Console.WriteLine($"{selectedItem.Name}을(를) {sellPrice}G에 판매했습니다.");
            }
            else
            {
                Console.WriteLine("판매할 수 없는 아이템입니다.");
            }

            Console.WriteLine("아무 키나 누르면, 상점으로 돌아갑니다.");
            Console.ReadKey();
            StoreMenu();
        }

        private static void ShowHighlightText(string text) // 6-1. 첫 줄 색 변경 함수, 마젠타 색
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private static void PrintTextWithHighlights(string s1, string s2, string s3 = "") // 6-2. 문자열 하이라이트 효과 함수 
        {
            Console.Write(s1);
            Console.ForegroundColor = ConsoleColor.Yellow; // 노란색 발동 -> s2에 적용
            Console.Write(s2);
            Console.ResetColor(); // 색 리셋
            Console.WriteLine(s3);
        }



        private static int CheckValidInput(int min, int max) // 5-6. 입력 값 유효성 확인 함수 (int 반환)
        {
            // 아래 두 가지 상황은 비정상, 재입력 수행
            // 1. 숫자가 아닌 입력을 받은 경우,    2. 숫자가 최솟값 에서 최댓값의 범위를 벗어난 경우
            int keyInput; // tryParse(정수화)에 필요
            bool result;  // while 반복문에 필요
            do // 일단 한 번 실행
            {
                Console.WriteLine("원하시는 행동을 입력 해주세요.");
                result = int.TryParse(Console.ReadLine(), out keyInput);
                // 입력을 정수로 변환하여 int KeyInput에 저장하고, 결과 값을 result로 설정.
                // 결과 값이 숫자(정수)면 가져오고, 그 외면 안 가져옴(실행 안한다는 뜻)
            }
            while (result == false || CheckIfVaild(keyInput, min, max) == false); // result가 false거나 CheckIfVaild함수가 false면 반복 

            //여기에 도착했다는 것은 (아래 유효성 확인 bool 함수를 통해) 제대로 입력을 받았다는 것.
            return keyInput;
        }

        private static bool CheckIfVaild(int keyInput, int min, int max) // 5-7. 유효성 확인 함수2 (bool 반환)
        {
            if (min <= keyInput && keyInput <= max) return true; // 키 입력값이 min ~ mix 사이면 return이 참 = 실행
            return false; // 그 외면 false
        }

        static void AddItem(Item item) // 5-3. 아이템 추가 함수
        {                               // ( ) 안에 Item item ? -> AddItem 메서드의 매개 변수 = 외부에서 Item 객체의 item 매개 변수를 받아 온 것.
                                        // Item 객체의 속성과 메서드에 접근 할 수 있음(데이터 읽기, 수정, 필요 기능 수행)
                                        // 타입 유연성, 재사용성, 안정성을 보장하고 잘못된 타입의 객체 전달 방지.

            if (Item.ItemCnt == 9) return; // Item클래스 객체의 ItemCnt 변수가 10이면 아무 것도 안 만든다.
            _items[Item.ItemCnt] = item;  // 0개 -> 0번 인덱스, 1개 -> 1번 인덱스
            Item.ItemCnt++;
        }

        private static void PrintStartLogo() // 5-4. 게임 스타트 화면
        {

            Console.WriteLine("==================================================================");
            Console.WriteLine("                            sparta dungeon holy edition                         ");
            Console.WriteLine("                                         ___                                    ");
            Console.WriteLine("                                        l   l                                   ");
            Console.WriteLine("                                 ________   ________                            ");
            Console.WriteLine("                                l                   l                           ");
            Console.WriteLine("                                l________   ________l                           ");
            Console.WriteLine("                                        l   l                                   ");
            Console.WriteLine("                                        l   l                                   ");
            Console.WriteLine("                                        l   l                                   ");
            Console.WriteLine("                                        l   l                                   ");
            Console.WriteLine("                                         ___                                    ");
            Console.WriteLine("==============================================================================");
            Console.WriteLine("                            PRESS ANYWAY TO START                             ");
            Console.WriteLine("==============================================================================");
            Console.ReadKey(); // 아무 키나 입력 받음
        }

        private static void RestMenu() // 9. 휴식하기
        {
            Console.Clear();
            ShowHighlightText("■ 휴 식 하 기 ■");
            Console.WriteLine("돈을 내고 여관에서 휴식을 하여 체력을 회복 시킬 수 있습니다.");
            Console.WriteLine("");
            Console.WriteLine($" 500 G 를 내면 체력을 회복할 수 있습니다. (보유 골드 : {_player.Gold} G)\n");
            Console.WriteLine("1. 휴식하기");
            Console.WriteLine("0. 나가기");

            string input = Console.ReadLine(); // 입력한 input은 문자열이다.
            if (input == "0") // 0번 나가기 입력 시 실행
            {
                StartMenu();
                return;
            }

            if (input == "1") // 휴식을 선택한 경우
            {
                if (_player.Hp == 100) // 이미 최대 체력인 경우
                {
                    Console.WriteLine("이미 최대 체력입니다.");
                }
                else if (_player.Gold >= 500) // 골드가 500이상이며, 최대 체력이 아닌 경우(생략)
                {
                    _player.Gold -= 500; // 골드 차감
                    _player.Hp = 100; // 체력을 최대(100)로 회복
                    Console.WriteLine($"휴식을 완료했습니다. (보유 골드 : {_player.Gold} G)");
                }
                else // 골드가 부족한 경우
                {
                    Console.WriteLine("Gold가 부족합니다.");
                }
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다.");
            }

            Console.WriteLine("\n아무 키나 누르면 돌아갑니다.");
            Console.ReadKey();
            StartMenu();
        }

        private static void DungeonMenu() // 12. 던전 입장
        {
            Console.Clear();
            ShowHighlightText("■ 던전 입장 ■");
            Console.WriteLine("이곳에서 던전으로 들어가기 전 다음 활동을 할 수 있습니다.");
            Console.WriteLine("");
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전 입장");
            Console.WriteLine("0. 메인 화면으로 나가기");
            Console.WriteLine("");

            switch (CheckValidInput(0, 4))
            {
                case 0:
                    StartMenu(); // 메인 메뉴로 나가기
                    break;
                case 1:
                    StatusMenu(); // 상태 보기
                    break;
                case 2:
                    InventoryMenu(); // 인벤토리
                    break;
                case 3:
                    StoreMenu(); // 상점
                    break;
                case 4:
                    DungeonChoiceMenu(); // 던전 난이도 선택
                    break;
            }
        }

        private static void DungeonChoiceMenu() // 12-1. 던전 난이도 선택
        {
            Console.Clear();
            ShowHighlightText("■ 던전 난이도 선택 ■");
            Console.WriteLine("다양한 난이도의 던전을 선택할 수 있습니다.");
            Console.WriteLine("");
            Console.WriteLine("1. 작은 동굴     | 방어력 5 이상 권장");
            Console.WriteLine("2. 버려진 성당     | 방어력 11 이상 권장");
            Console.WriteLine("3. 지옥의 문   | 방어력 17 이상 권장");
            Console.WriteLine("0. 던전 입구로 돌아가기");
            Console.WriteLine("");

            if (_player.Hp <= 0) // 체력이 0이하인 경우 입장컷.
                if (_player.Hp <= 0) // 체력이 0이하인 경우 입장컷.
                {
                    Console.Clear();
                    Console.WriteLine("!! 체력이 부족합니다. 회복 후에 도전해주세요 !!");
                    Console.WriteLine("");
                    Console.WriteLine("0. 던전 입구로 돌아가기");
                    CheckValidInput(0, 0);
                    DungeonMenu();
                    return;
                }

            switch (CheckValidInput(0, 3))
            {
                case 0:
                    DungeonMenu(); // 던전 메뉴
                    break;
                case 1:
                    ExecuteDungeon("작은 동굴", 5, 1000); // ExecuteDungeon의 매개변수 값 입력 ( 던전 이름, 요구 방어력, 보상 골드 ) 
                    break;
                case 2:
                    ExecuteDungeon("버려진 성당", 11, 1700);
                    break;
                case 3:
                    ExecuteDungeon("지옥의 문", 17, 2500);
                    break;

                    // 처음에는 EasyDungeon(); NormalDungeon(); HardDungeon(); 으로 구성했는데
                    // 메서드가 하나여도, 로직에 따라 난이도 차이를 줄 수 있어서 ExecuteDungeon() 하나로 합침. 
            }
        }

        private static void ExecuteDungeon(string dungeonName, int requiredDef, int reward)
        {
            Console.WriteLine($"{dungeonName}에 입장합니다...");
            Console.WriteLine("");

            int totalDef = _player.Def + getSumBonusDef(); // 총 방어력 = 플레이어 방어력 + 아이템 추가 방어력
            int hpLoss = new Random().Next(20, 36); // 무작위 체력 감소 20 ~ 35
            int bonusReward = reward; // 기본 보상 설정 + 공격력에 따른 추가 보상이 더 해질 수 있게 정의

            double perBonusReward = new Random().Next(_player.Atk, _player.Atk * 2) / 100.0;
            // 플레이어의 공격력에 따른 추가 보상의 비율을 결정.
            // 공격력의 1 ~ 2배 사이의 값을 백분율 % 로 계산
            // double 사용 이유 -> float보다 정밀함
            // new Random().Next() 는 난수(무작위 수)를 생성하는 데 사용, Next의 매개변수에 지정 해준 값 2개가 랜덤 범위임.

            if (totalDef < requiredDef) // 총 방어력이 권장 방어력보다 낮은 경우
            {
                if (new Random().NextDouble() < 0.4) // 랜덤 조건문, 40% 확률로 실패..    = 40% 확률로 작동
                {
                    Console.WriteLine("던전을 클리어하지 못했습니다.. 체력이 반 감소합니다.");
                    _player.Hp = Math.Max(_player.Hp - hpLoss / 2, 0); // 실패시 체력이 절반으로 감소, 0보다는 낮아지지 않음.
                    _player.Hp = Math.Min(_player.Hp, 100);
                    // Math.Max(a,b) -> a,b를 비교해서 더 큰 값을 반환, 
                    // 체력이 0 이하로 떨어지지 않도록 하려고 사용. ( 0이 음수보다 더 크므로 0이 반환 )
                }
                else // 권장 방어력 이상이거나, 권장 방어력 이하이지만 던전 실패가 아닌 경우
                {
                    Console.WriteLine("던전을 클리어했습니다! 축하 드립니다.");

                    bonusReward += (int)(reward * perBonusReward);
                    // bonusReward 보너스 골드는  기본 골드 x 추가 공격력 비율 보상 곱해서 더 함
                    // (결과값 int형으로)

                    _player.Hp = Math.Max(_player.Hp - hpLoss, 0); // 플레이어 체력 - 손실 체력,  0보다는 낮아지지 않음.
                    _player.Hp = Math.Min(_player.Hp, 100);
                    _player.Gold += bonusReward; // 플레이어 골드 + 보너스 골드
                }
            }
            else // 권장 방어력 이상인 경우
            {
                Console.WriteLine("던전을 클리어했습니다!");
                hpLoss -= totalDef - requiredDef; // 체력손실량 = 방어력에 따라 체력 감소량 조정  

                bonusReward += (int)(reward * perBonusReward); // 위와 동일

                _player.Hp = Math.Max(_player.Hp - (hpLoss > 0 ? hpLoss : 0), 0);
                // 삼항 연산자, hpLoss가 0보다 크면 hpLoss를 쓰고, 아니면 0을 사용
                // 즉 체력 손실이 0보다 크면 그 값을 체력에서 차감하고, 그렇지 않으면 차감 X
                // Math.Max로, 체력 계산값이 0보다 크면 그 값을 _player.Hp로 반환 = 음수 방지

                _player.Hp = Math.Min(_player.Hp, 100);
                // 그런데, 이번엔 체력이 100을 초과하는 경우가 발생해서 Math.Min(더 작은 녀석 반환)를 만들어 줬음. 다른 조건문에도 만들어야 함.

                _player.Gold += bonusReward; // 플레이어의 골드에 최종 보상 더하기
            }

            Console.WriteLine("\n[탐험 결과]");

            Console.WriteLine($"체력 {_player.Hp + hpLoss} -> {_player.Hp}");
            // {_player.Hp + hpLoss} = 던전 전 체력

            Console.WriteLine($"Gold + {reward} G -> {_player.Gold} G\n");
            // 얼마가 추가되었는지 알 수 있 도록 {reward} 표시

            Console.WriteLine("0. 나가기");

            CheckValidInput(0, 0);
            StartMenu();
        }

        private static void GameOverMenu() // 11. 게임 종료
        {
            Console.Clear();
            ShowHighlightText("■ 게임을 종료하시겠습니까? ■");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("1. 게임을 종료합니다.\n");
            Console.WriteLine("0. 돌아갑니다");

            switch (CheckValidInput(0, 1))
            {
                case 1: // 게임 종료
                    Console.WriteLine("아무 키나 누르면, 게임을 종료합니다.");
                    Console.ReadKey();
                    Environment.Exit(0); // 애플리케이션 종료
                    break;
                case 0: // 메인 메뉴로 돌아가기
                    StartMenu();
                    break;
            }
        }

        private static void SaveGameMenu() // 10. 게임 저장
        {
            Console.Clear();
            ShowHighlightText("■ 저장 및 불러오기 ■");
            Console.WriteLine("게임을 저장하거나, 불러올 수 있습니다.");
            Console.WriteLine("");
            Console.WriteLine("1. 게임 저장하기");
            Console.WriteLine("2. 게임 불러오기");
            Console.WriteLine("0. 메인 화면으로 나가기");
            Console.WriteLine("");
            var input = Console.ReadLine();
            switch (input)
            {
                case "0":
                    StartMenu();
                    break;
                case "1":
                    SaveGame();
                    Console.WriteLine("게임이 저장되었습니다! 아무 키나 눌러주시면 메인 메뉴로 돌아갑니다.");
                    Console.ReadKey();
                    StartMenu(); // 메인 메뉴로
                    break;
                case "2":
                    LoadGame();
                    Console.WriteLine("게임을 불러왔습니다! 아무 키나 눌러주시면 메인 메뉴로 돌아갑니다.");
                    Console.ReadKey();
                    StartMenu();
                    break;

                default:
                    Console.WriteLine("잘못된 입력입니다.");
                    break;
            }
        }
        // 저장하기 전에, 패키지 관리자 콘솔에서 Install-Package Newtonsoft.Json 설치
        static void SaveGame() // 10-1. 게임 저장
        {
            //FIX
            string Player = JsonSerializer.Serialize(_player);
            string item = JsonSerializer.Serialize(_items);
            File.WriteAllText(FilePath + "Player.json", Player);
            File.WriteAllText(FilePath + "Item.json", item);
        }
        static void LoadGame() // 10-2. 게임 불러오기
        {
            string playerData = File.ReadAllText(FilePath + "Player.json");
            string ItemData = File.ReadAllText(FilePath + "Item.json");
            _player = JsonSerializer.Deserialize<Character>(playerData) ?? new Character("x", "x", 0, 0, 0, 0, 0);
            _items = JsonSerializer.Deserialize<Item[]>(ItemData) ?? new Item[0];
        }

        
    }
}