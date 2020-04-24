# Xây dựng một APIs dùng để quản lý Product code và Genarate QRcode.
## Kiến thiết kỹ thuật
Sử dụng RESHful HTTP để build API.
URLs chính của APIs: **/APIs/v0/MasterCodes**
Method GET: APIs/v0/MasterCodes
Trả về danh sách các APIs và hướng dẫn kèm theo

# Các Methods tài nguyên:
## ProCodes
	GET: APIs/v0/MasterCodes/GroupCodes
	Return: Danh dách các nhóm code đã tạo

	GET: APIs/v0/MasterCodes/GroupCodes/<id>
	Return: Danh dách các nhóm code đã tạo
	
## Find
	***GET: APIs/v0/MasterCodes/GroupCodes/?q=<field1:content1>,<field2:content2>,<..n>***
	Mô tả: Tìm danh sánh code theo điều kiện field và content. Danh sách những fields hiện có Id, GroupName, UserCode, AGICode, Quantity,DataTime
	Return: danh sách trả về theo điều kiện
	VD: 	Tìm danh sách được tạo bởi nhân nhân viên có mã số NV123  
			APIs/v0/MasterCodes/GroupCodes/?q=<UserCode:NV123>
		Nếu kèm theo đó là điều kiện ngày tháng ta có:
			APIs/v0/MasterCodes/GroupCodes/?q=UserCode:NV123,DateTime:20-04-30
## Create
	GET: APIs/v0/MasterCodes/GroupCodes
	Mô tả: Tạo danh sánh code và lưu những thông tin kèm theo. Client phải gửi những đoạn mã Json có chứa thông tin: Quantity, UserCode, AGICode trong body request.
	Return: Status và thông tin vừa tạo.

>Anh Khoa