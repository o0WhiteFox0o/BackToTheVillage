# Welcome

---

# Contents
[Nhân vật](#characters)  
[Cây trồng](#plants)  
[Cá](#fish)  
[Động vật](#animals)  
[Vật phẩm](#items)  
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

[Công trình](#constructs)  

---


# Characters
> ## <i class="fa-solid fa-pencil"></i> Main Character set up
> Nhân vật chính di chuyển theo 4 hướng (Đông Bắc, Tây Bắc, Đông Nam, Tây Nam).
>
> ## <i class="fa-solid fa-clone"></i> Add a NPC
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
> 
---

# Items
> ## Overview
> ### Chuẩn bị
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
> 
---
> 
> ## Tools
> ### Chuẩn bị
> Các thành phần cẩn chuẩn bị cho Tools Item:
> - **Sprite Icon**: Đặt trong thư mục *Assets/Sprites/Items/Icons/Tools*.
> 
> ### Thiết lập Tools Item
> Các thành phần chính của Tools Item:
> - Các thành phần từ [Item cơ bản](#items).
> 
> Vị trí lưu trữ:
> - Scriptable Object Tools Item được lưu trong *Assets/Resources/Items/Tools*.
> 
> 
---
> 
> ## Materials
> ### Chuẩn bị
> Các thành phần cẩn chuẩn bị cho Materials Item:
> - **Sprite Icon**: Đặt trong thư mục *Assets/Sprites/Items/Icons/Materials*.
> 
> ### Thiết lập Material Item
> Các thành phần chính của Materials Item:
> - Các thành phần từ [Item cơ bản](#items).
> 
> Vị trí lưu trữ:
> - Scriptable Object Materials Item được lưu trong *Assets/Resources/Items/Materials*.
> 
> 
---
> 
> ## Crafting
> ### Chuẩn bị
> Các thành phần cẩn chuẩn bị cho Crafting Item:
> - **Sprite Icon**: Đặt trong thư mục *Assets/Sprites/Items/Icons/Crafting*.
> 
> ### Thiết lập Crafting Item
> Các thành phần chính của Crafting Item:
> - Các thành phần từ [Item cơ bản](#items).
> 
> Vị trí lưu trữ:
> - Scriptable Object Crafting Item được lưu trong *Assets/Resources/Items/Crafting*.
> 
> 
---
> 
> ## Crops
> ### Chuẩn bị
> Các thành phần cẩn chuẩn bị cho Crops Item:
> - **Sprite Icon**: Đặt trong thư mục *Assets/Sprites/Items/Icons/Crops*.
> 
> ### Thiết lập Crops Item
> Các thành phần chính của Crops Item:
> - Các thành phần từ [Item cơ bản](#items).
> 
> Vị trí lưu trữ:
> - Scriptable Object Crops Item được lưu trong *Assets/Resources/Items/Crops*.
> 
> 
---
> 
> ## Fish
> ### Chuẩn bị
> Các thành phần cần chuẩn bị để thiết lập cho Fish Item:
> - **Sprite Icon**: Đặt trong thư mục *Assets/Sprites/Items/Icons/Fish*.
> ### Thiết lập Fish Item
> ![Các thành phần trong Fish Item](./README_Images/FishItem_Setup.png)  
> 
> Các thành phần chính của Fish Item:
> - Các thành phần từ [Item cơ bản](#items).
> - **Thông tin cơ bản**:
>   - **Fish Name**: Tên loại cá.
> - **Độ khó QTE**:
>   - **QTE Bar Speed**: Tốc độ quay của QTE bar.
>   - **Success Window Size**: 
>   - **Max Game Time**: Thời gian câu tối đa.
>   - **Progress Increase**: Số điểm được cộng khi nhấn trúng.
>   - **Progress Decrease**: Số điểm bị trừ khi nhấn trượt.  
> 
> Vị trí lưu trữ:
> - Scriptable Object Fish Item được lưu trong *Assets/Resources/Items/Fish*.
> 
> 
---
> 
> ## Animal products
> ### Chuẩn bị
> Các thành phần cẩn chuẩn bị cho Animal products Item:
> - **Sprite Icon**: Đặt trong thư mục *Assets/Sprites/Items/Icons/Animal_Products*.
> 
> ### Thiết lập Animal products Item
> Các thành phần chính của Animal products Item:
> - Các thành phần từ [Item cơ bản](#items).
> 
> Vị trí lưu trữ:
> - Scriptable Object Animal products Item được lưu trong *Assets/Resources/Items/Animal_Products*.
> 
> 
---
> 
> ## Food
> ### Chuẩn bị
> Các thành phần cẩn chuẩn bị cho Food Item:
> - **Sprite Icon**: Đặt trong thư mục *Assets/Sprites/Items/Icons/Food*.
> 
> ### Thiết lập Food Item
> Các thành phần chính của Food Item:
> - Các thành phần từ [Item cơ bản](#items).
> 
> Vị trí lưu trữ:
> - Scriptable Object Food Item được lưu trong *Assets/Resources/Items/Food*.
> 
> 
---
> 
> ## Decoration
> ### Chuẩn bị
> Các thành phần cẩn chuẩn bị cho Decoration Item:
> - **Sprite Icon**: Đặt trong thư mục *Assets/Sprites/Items/Icons/Decoration*.
> 
> ### Thiết lập Decoration Item
> Các thành phần chính của Decoration Item:
> - Các thành phần từ [Item cơ bản](#items).
> 
> Vị trí lưu trữ:
> - Scriptable Object Decoration Item được lưu trong *Assets/Resources/Items/Decoration*.
> 
> 
---
> 
> ## Other
> 
---

# Constructs
> 
