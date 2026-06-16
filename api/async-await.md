- C# là thread pool. Khi một request vào thì lấy 1 thread thực thi
- Async, await thì dùng để kết nối I/O với bên ngoài và chờ đợi
- Trong lúc chờ đợi thì nó trả thread về thread pool, khi nào xong thì lấy 1 thread vào thực thi tiếp 
- Khi đó các hàm sử dụng await của các librabries hỗ trợ thì tên hàm phải thêm vào chữ Async ở cuối 

- Task là kiểu return type when function use async/ await: 
    1. Task không trả về dữ liệu thì cứ giữ nguyên chữ Task
    2. Task<T> trả về dữ liệu bất kỳ 
    3. Trong Api thường dùng Task<IActionResult>
- Task giống như promise, khi có nhiều cái muốn chạy thì dùng Task.WhenAll()

# Cancellation: là một lá cờ báo hủy 
- Vidu: Truy vấn db mất 30s nhưng 2s sau thì có vấn đề (client đóng tab) thì ko được để nó chạy ngầm như thế 
-> dùng cancellation để báo hủy 
- Cách dùng: 
  - Cancellation là cơ chế “xin dừng” (cooperative). Nó KHÔNG tự kill thread; code/ thư viện phải hỗ trợ và tôn trọng token.
  - CancellationToken (ct) là “lá cờ” báo hủy, CancellationTokenSource (cts) là “nguồn” phát lệnh hủy (Cancel()).

## 1) Nhận token từ ASP.NET Core Request (thường gặp nhất)
- Trong Controller, chỉ cần thêm tham số `CancellationToken ct` là framework tự bind token gắn với request (client hủy request thì ct sẽ bị cancel):

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetUser(int id, CancellationToken ct)
{
    var user = await _service.GetUserAsync(id, ct);
    if (user is null) return NotFound();
    return Ok(user);
}
```

## 2) Truyền ct xuống các API có hỗ trợ cancellation (quan trọng nhất)
- Token “có sẵn” nhưng nếu bạn không truyền nó xuống thì cancellation không có tác dụng.
- Ví dụ với EF Core / Task.Delay / HttpClient:

```csharp
await Task.Delay(500, ct);
var users = await _db.Users.ToListAsync(ct);
var resp = await _http.SendAsync(req, ct);
```

## Nếu bạn KHÔNG truyền `CancellationToken` thì sao?
- Thường là **KHÔNG lỗi**: code vẫn chạy như bình thường.
- Nhưng **không cancel được**: client có đóng tab/hủy request thì DB call / HTTP call của bạn vẫn có thể chạy tiếp tới khi xong.
- Trường hợp “bị lỗi” thường là **lỗi compile** (thiếu tham số) khi bạn gọi method/overload bắt buộc có `CancellationToken`.

## 3) Tự tạo cancellation / timeout bằng CancellationTokenSource
- Khi bạn muốn chủ động hủy hoặc đặt timeout cho 1 đoạn xử lý / 1 request gọi ra ngoài:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
var resp = await _http.GetAsync("https://example.com", cts.Token);
```

## 4) Tự kiểm tra token trong xử lý dài (loop, import/export, xử lý file lớn)
- Với code chạy lâu “thuần CPU/logic”, bạn tự check để dừng sớm:

```csharp
for (var i = 0; i < 1_000_000; i++)
{
    ct.ThrowIfCancellationRequested();
    // ... xử lý ...
}
```

---

## Ghi chú nhỏ về `Task<IActionResult>` (API hay dùng)
- Chuẩn là `Task<IActionResult>` (I + Action + Result).
- Dùng trong ASP.NET Core Controller khi action async và bạn muốn trả về các kết quả HTTP như `Ok()`, `BadRequest()`, `NotFound()`, `Created()`...
- Ngoài ra hay gặp `Task<ActionResult<T>>` khi muốn “type rõ” dữ liệu trả về (ví dụ `Task<ActionResult<UserDto>>`).
