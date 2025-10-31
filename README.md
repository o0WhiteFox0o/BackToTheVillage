# Welcome

---

# Contents
- [Danh sách bảng](#table-list)
- [Hướng dẫn lập trình](#programming-guide)  
    - [Cấu trúc thư mục trong Unity](#unity-folder-structure)  
    - [Hướng dẫn tổ chức Scene](#scene-organization-guidelines)
    - [Hướng dẫn lập trình](#coding-guideline)  

- [Nhân vật](#characters)  
- [Cây trồng](#plants)  
- [Cá](#fish)  
- [Động vật](#animals)  
- [Vật phẩm](#items)  
    - [Tổng quan](#overview)
    - [Công cụ](#tools)
    - [Nguyên liệu](#materials)
    - [Chế tạo](#crafting)
    - [Nông sản](#crops)  
    - [Cá](#fish)
    - [Sản phẩm động vật](#animal-products)  
    - [Thức ăn](#food)
    - [Trang trí](#decoration)
    - [Vật phẩm khác](#other)  

- [Công trình](#constructs)  
- [Dữ liệu](#data)
    - [Dữ liệu chung](#game-config)
    - [Dữ liệu nông trại](#farm-config)

---

# Table List
- [Bảng quy tắc đặt tên file](#files-name-guidelines-table)
- [Bảng quy tắc đặt tên trong code](#naming-guides-table)

---

# Programming Guide
> ## Unity Folder Structure
> Cây thư mục của dự án được tổ chức dưới dạng:  
> **Assets/** - Thư mục chính của dự án, chứa các thành phần trong trò chơi.
> - **Art/** - Chứa các thành phần đồ họa của trò chơi như model, animation, ...
>   - **Models/** - Chứa các model được dùng trong dự án.
>       - **Items/** - 
>           - **Icons/** - Chứa các sprite icon của vật phẩm, tên các sprite icon được đặt theo dạng ***ICON_<tên_vật_phẩm>***.
>               - **Tools/**
>               - **Materials/**
>               - **Crafting/**
>               - **Crops/**
>               - **Food/**
>               - **Animal_Products/**
>               - **Decorations/**
>               - **Fish/**
>               - **Others/**
>           - **Sprites** - Chứa sprite cho các vật phẩm trong trò chơi, bao gồm các thư mục con tương tự trong thư mục Assets/Art/Models/Items/Icons/.
>   - **Animations/**
>       - **Animation Clips/** - Chứa các file animation clip, được đặt tên theo dạng ***ANIM_<tên_animation>***.
>       - **Animator Controllers/** - Chứa các file animator controller, đặt tên theo dạng ***AC_<tên_animator>***.
>   - **VFX/** - Chứa các hiệu ứng hình ảnh cho trò chơi, đặt tên file theo dạng ***VFX_<tên_hiệu_ứng>***.
> - **Audio/**  
>   - **Music/** - Chứa các file nhạc trong trò chơi, quy tắc đặt tên ***MUS_<tên_nhạc>***.
>   - **SFX/** - Chứa các file hiệu ứng âm thanh trong trò chơi, quy tắc đặt tên ***SFX_<tên_âm_thanh>***.
> - **Prefabs/** - Chứa các prefab dùng trong dự án, có thể chia ra làm nhiều thư mục con để quản lý, quy tắc đặt tên ***PFB_<tên_prefab>***.
> - **Scenes/** - Chứa các màn chơi trong trò chơi, quy tắc đặt tên ***SCN_<tên_scene>***.
> - **Scripts/** - Chứa các file lệnh trong dự án.
>   - **Core/** - Chứa các lệnh về game logic, quy tắc đặt tên ***C_<tên_script>***.
>   - **User_Interfaces/** - Chứa các lệnh liên quan đến UI của trò chơi, quy tắc đặt tên ***UI_<tên_script>***.
>   - **Managers/** - Chứa các lệnh liên quan đến quản lý các đối tượng trong trò chơi, quy tắc đặt tên ***MGR_<tên_script>***.
>   - **Scriptable_Obj** - Chứa các file script của các scriptable object, quy tắc đặt tên ***SO_<tên_script>***.
>   - **Systems/** - Chứa các file lệnh liên quan đến hệ thống trò chơi, quy tắc đặt tên ***SYS_<tên_script>***.
> - **UI/**  
> - **Resources/** - Chứa các file scriptable object về dữ liệu của các thành phần trong trò chơi, quy tắc đặt tên ***RES_<tên_file>***.
>   - **Items/** - *Chứa các Scriptable object của vật phẩm có trong trò chơi, bao gồm các thư mục con được liệt kê trong thư mục Assets/Art/Models/Items/Icons/*.
> 
> ---
> 
> ### Files name guidelines table
> | Loại file | Quy tắc đặt tên |
> | --------- | --------------- |
> | File sprite icon của vật phẩm | ICON_<tên_file> |
> | File animation clip | ANIM_<tên_animation> |
> | File animator controller | AC_<tên_file> |
> | File visual effect | VFX_<tên_file> |
> | File nhạc | MUS_<tên_file> |
> | File sound effect | SFX_<tên_file> |
> | Prefab | PFB_<tên_prefab> |
> | Scene | SCN_<tên_scene> |
> | Script logic game (Core) | C_<tên_script> |
> | Script UI | UI_<tên_script> |
> | Script quản lý | MGR_<tên_script> |
> | Scriptable object | SO_<tên_script> |
> | Script hệ thống trò chơi | SYS_<tên_script> | 
> | File Resources | RES_<tên_file> |
> 

---

> ## Scene Organization Guidelines
>
> Cách thức tổ chức Scene trong dự án  
>> **SCENE**
>> - **SYSTEMS**
>>   - **Game_Manager** - Chứa các thành phần quản lý chung của scene/trò chơi.
>>   - **Audio_Manager** - Chứa các thành phần quản lý âm thanh của scene/trò chơi.
>>   - **UI_Manager** - Chứa các thành phần quản lý giao diện của scene/trò chơi.
>>   - **Input_Manager** - Chứa các thành phần quản lý đầu vào của scene/trò chơi.
>>   - Others system manager ...
>> - **ENVIRONMENT**
>>   - **Isometric_Grid** - Chứa bản đồ isometric của màn chơi.
>>       - **Ground** - Chứa bản đồ isometric của nền đất (đất, đường đi, nước, ...)
>>       - **Fishing** - Chứa các ô đánh dấu khu vực nước có thể câu cá.
>>       - **Decoration** - Chứa các đối tượng trang trí trong bản đồ.
>>   - **Lighting**
>>   - **Buildings** - Chứa các đối tượng công trình trong màn chơi.
>> - **INTERACTABLE**
>>   - **Collectibles** - Chứa các đối tượng có thể thu thập được trong màn chơi.
>>   - **Triggers** - 
>>   - **Door** - 
>> - **CHARACTERS**
>>   - **Player**
>>   - **NPCs**
>> - **VFX**
>>   - **Environment_FX**
>>   - **Character_FX**
>>   - **Event_FX**
>> - **AUDIO**
>>   - **Ambiant_Sound**
>>   - **Environment_SFX**
>>   - **Event_SFX**
>> - **UI**
>>   - **Canvas_Main**
>>   - **Canvas_Pause**  
>  
> 

---

> ## Coding Guidelines
> 
> **Quy ước cách trình bày**
> - Dấu ngoặc nhọn: Cả dấu ngoặc đóng và ngoặc mở phải được đặt tại dòng mới.
> - Sau mỗi dòng lệnh bắt đầu cấu trúc, “mức” tăng lên 1 tab.
> - Sau mỗi dòng lệnh kết thúc cấu trúc, “mức” giảm đi 1 tab.
> - Các dòng lệnh cùng “mức” thụt vào đều nhau.
> - Các “mức” thụt vào cách đều nhau.
> - Ví dụ  
> ![Cách trình bày](./README_Images/standard_code_syntax.png)
> 
> - Khi một biểu thức dài hơn một dòng đơn, cần ngắt chúng theo nguyên tắc sau: 
>   - Ngắt sau dấu phẩy
>   - Ngắt trước toán tử
>   - Những quy ước ngắt có độ ưu tiên cao hơn sẽ được ưu tiên trước
>   - Dòng mới được thụt vào 2 tab 
>   - Với một biểu thức dài có sử dụng dấu ngặc đơn thì không nên ngắt dòng trong cặp dấu ngoặc đơn “( )”
>   - Ngắt dòng cho biểu thức if nên sử dụng quy tắc 2 tab do việc sử dụng 1 tab làm cho phần body của biểu thức khó theo dõi
> - Ví dụ  
> ![Quy ước ngắt dòng](./README_Images/break_line_guide.png)
> 
> ---
> 
> **Comments**
> 
> Các block comment được dùng để mô tả files, được đặt ở đầu của mỗi file. Một block comment thì trước nó phải được đặt một dòng trống để phân cách nó với phần còn lại của code, tránh bị nhầm lẫn.  
> Quy ước cách viết block comment đặt ở đầu file  
> ![Block comment](./README_Images/block_comment.png)  
> 
> Documentation comment dùng để mô tả các class, interfaces, constructors, methods, và fields. Được đặt ở đầu class, interface, ...  
> Quy ước cách viết documentation comment  
> ![Documentation comment](./README_Images/documentation_comment.png)
> 
> ---
> 
> **Khai báo**
> 
> Chỉ nên sử dụng mỗi khai báo trên một dòng.  
> Chỉ đặt các khai báo ở phần đầu của các block (class, method). Không nên đợi đến khi nào cần sử dụng rồi mới khai báo vì điều này có thể gây ra sự rối rắm.  
> 
> ---
> 
> **Quy ước đặt tên**
> 
> Người lập trình khi đặt tên phải có ý nghĩa và phải chỉ ra được mục đích của file / variable / control / method.  
> Nên tránh sử dụng các tên gần giống nhau.  
> Không nên sử dụng những tên khó hiểu, kể cả trong trường hợp nó chỉ làm biến đệm hoặc làm biến đếm.  
>   
> ### Naming guides table
> | Loại | Quy ước |
> | ---- | ------- |
> | Class/Interface | Bắt đầu bằng chữ in hoa.<br> Các Interfaces không có method nào thì nên thêm chữ I ở đầu |
> | Biến (Variables) | Phải được bắt đầu với một ký tự thường.<br> Các biến List nên thêm vào cuối chữ List. <br> Các biến Array có thể thêm vào cuối chữ Array. <br> ... |
> | Hằng (Constants) | Mọi hằng số phải được viết hoa tất cả các chữ và giữa các từ được liên kết với nhau bằng dấu gạch dưới.<br> Mọi hằng số phải được khai báo static. |
> | Phương thức (Methods) | Tên method phải bắt đầu bằng một chữ viết hoa.<br> Từ đầu tiên của tên method nên sử dụng “động từ”. |
> 

---

# Characters
> ## Main Character set up
> Nhân vật chính di chuyển theo 4 hướng (Đông Bắc, Tây Bắc, Đông Nam, Tây Nam).
>
> ---
> 
> ## Add a NPC
> 
> ![NPC Data Setup](./README_Images/npc_data_setup.png)
> 

---

# Plants
> 
> ## Add a plant

---

# Fish
> 
> ## Add a fish

---

# Animals
> 

---

# Items
> ## Overview
> ### Chuẩn bị
> 
> ---
> 
> ### Thiết lập vật phẩm
> ![Các thành phần của vật phẩm](./README_Images/Item_Setup.png)  
>
> Các thành phần chính của một vật phẩm:
> - **Overview**:
>   - **Id**: Mã của vật phẩm.
>   - **Icon**: Hình ảnh của vật phẩm đó (icon trong kho đồ).
>   - **Stackable**: Vật phẩm có thể cộng dồn trong kho đồ được hay không.
>   - **Can Sell**: Vật phẩm có thể bán được hay không.
> - **Price**:
>   - **Buy Price**: Giá mua vào của vật phẩm _(Nếu vật phẩm không thể mua được thì đặt giá trị này là -1)_.
>   - **Sell Price**: Giá bán ra của vật phẩm _(Nếu vật phẩm không thể bán được thì đặt giá trị này là -1)_. 
> 

---

> ## Tools
> ### Chuẩn bị
> Các thành phần cẩn chuẩn bị cho Tools Item:
> - **Sprite Icon**: Đặt trong thư mục *Assets/Art/Models/Items/Icons/Tools*.
> 
> ---
> 
> ### Thiết lập Tools Item
> Các thành phần chính của Tools Item:
> - Các thành phần từ [Item cơ bản](#items).
> 
> Vị trí lưu trữ:
> - Scriptable Object Tools Item được lưu trong *Assets/Resources/Items/Tools*.
> 

---

> ## Materials
> ### Chuẩn bị
> Các thành phần cẩn chuẩn bị cho Materials Item:
> - **Sprite Icon**: Đặt trong thư mục *Assets/Art/Models/Items/Icons/Materials*.
>  
> ---
> 
> ### Thiết lập Material Item
> Các thành phần chính của Materials Item:
> - Các thành phần từ [Item cơ bản](#items).
> 
> Vị trí lưu trữ:
> - Scriptable Object Materials Item được lưu trong *Assets/Resources/Items/Materials*.
> 

---
 
> ## Crafting
> ### Chuẩn bị
> Các thành phần cẩn chuẩn bị cho Crafting Item:
> - **Sprite Icon**: Đặt trong thư mục *Assets/Art/Models/Items/Icons/Crafting*.
>  
> ---
> 
> ### Thiết lập Crafting Item
> Các thành phần chính của Crafting Item:
> - Các thành phần từ [Item cơ bản](#items).
> 
> Vị trí lưu trữ:
> - Scriptable Object Crafting Item được lưu trong *Assets/Resources/Items/Crafting*.
> 
 
---
 
> ## Crops
> ### Chuẩn bị
> Các thành phần cẩn chuẩn bị cho Crops Item:
> - **Sprite Icon**: Đặt trong thư mục *Assets/Art/Models/Items/Icons/Crops*.
>  
> ---
> 
> ### Thiết lập Crops Item
> Các thành phần chính của Crops Item:
> - Các thành phần từ [Item cơ bản](#items).
> 
> Vị trí lưu trữ:
> - Scriptable Object Crops Item được lưu trong *Assets/Resources/Items/Crops*.
> 
 
---
 
> ## Fish
> ### Chuẩn bị
> Các thành phần cần chuẩn bị để thiết lập cho Fish Item:
> - **Sprite Icon**: Đặt trong thư mục *Assets/Art/Models/Items/Icons/Fish*.
>  
> ---
> 
> ### Thiết lập Fish Item
> ![Các thành phần trong Fish Item](./README_Images/FishItem_Setup.png)  
> 
> Các thành phần chính của Fish Item:
> - Các thành phần từ [Item cơ bản](#items).
> - **Thông tin cơ bản**:
>   - **Fish Name**: Tên loại cá.
> - **Độ khó QTE**:
>   - **QTE Bar Speed**: Tốc độ quay của QTE bar.
>   - **Success Window Size**: Vùng màu xanh quay tròn của minigame.
>   - **Max Game Time**: Thời gian câu tối đa.
>   - **Progress Increase**: Số điểm được cộng khi nhấn trúng.
>   - **Progress Decrease**: Số điểm bị trừ khi nhấn trượt.  
> 
> Vị trí lưu trữ:
> - Scriptable Object Fish Item được lưu trong *Assets/Resources/Items/Fish*.
> 
 
---
 
> ## Animal products
> ### Chuẩn bị
> Các thành phần cẩn chuẩn bị cho Animal products Item:
> - **Sprite Icon**: Đặt trong thư mục *Assets/Art/Models/Items/Icons/Animal_Products*.
>  
> ---
> 
> ### Thiết lập Animal products Item
> Các thành phần chính của Animal products Item:
> - Các thành phần từ [Item cơ bản](#items).
> 
> Vị trí lưu trữ:
> - Scriptable Object Animal products Item được lưu trong *Assets/Resources/Items/Animal_Products*.
> 
 
---
 
> ## Food
> ### Chuẩn bị
> Các thành phần cẩn chuẩn bị cho Food Item:
> - **Sprite Icon**: Đặt trong thư mục *Assets/Art/Models/Items/Icons/Food*.
>  
> ---
> 
> ### Thiết lập Food Item
> Các thành phần chính của Food Item:
> - Các thành phần từ [Item cơ bản](#items).
> 
> Vị trí lưu trữ:
> - Scriptable Object Food Item được lưu trong *Assets/Resources/Items/Food*.
> 
 
---
 
> ## Decoration
> ### Chuẩn bị
> Các thành phần cẩn chuẩn bị cho Decoration Item:
> - **Sprite Icon**: Đặt trong thư mục *Assets/Art/Models/Items/Icons/Decoration*.
>  
> ---
> 
> ### Thiết lập Decoration Item
> Các thành phần chính của Decoration Item:
> - Các thành phần từ [Item cơ bản](#items).
> 
> Vị trí lưu trữ:
> - Scriptable Object Decoration Item được lưu trong *Assets/Resources/Items/Decoration*.
> 
 
---
 
> ## Other
 
---

# Constructs
> 

---

# Data
> ## Game Config
> Vị trí lưu trữ: *Assets/StreamingAssets/GameConfig.json*
> Các thành phần được lưu trữ:

---

> ## Farm Config
> Vị trí lưu trữ: *Assets/StreamingAssets/SavedFarms/<farm_name>.json*