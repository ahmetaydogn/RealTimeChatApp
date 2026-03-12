# RealTimeChatApp — Progress Tracker

## ✅ Tamamlanan

### Backend (ASP.NET Core)
- [x] N-Layer mimari kurulumu (Api, Business, DataAccess, Entities, Core)
- [x] Entity Framework Core kurulumu + migrations
- [x] BaseEntity (Guid Id, CreatedAt)
- [x] AppUser, Room, RoomMember, Message entity'leri
- [x] EfEntityRepositoryBase generic CRUD
- [x] IEntityTypeConfiguration yapısı (ApplyConfigurationsFromAssembly)
- [x] JWT Authentication (kayıt, giriş, token üretimi)
- [x] Redis bağlantısı (StackExchange.Redis)
- [x] SSE endpoint (`GET /api/chat/stream/{roomId}`)
- [x] Mesaj gönderme + Redis Pub/Sub (`POST /api/chat/send`)
- [x] Mesaj geçmişi — Redis cache (son 50) + DB fallback (`GET /api/chat/history/{roomId}`)
- [x] Room oluşturma + listeleme (`POST /api/room/PostRoom`, `GET /api/room/GetRooms`)
- [x] Swagger JWT desteği

---

## 🔄 Yapılacaklar

### Backend
- [x] Kullanıcı odaya katılma/ayrılma sistemi
  - `POST /api/room/join/{roomId}`
  - `POST /api/room/leave/{roomId}`
  - RoomMember tablosuna kayıt
  - SSE üzerinden odadaki kullanıcılara bildirim
- [ ] AdminOnly room kontrolü
  - Mesaj göndermeden önce kullanıcının RoomRole kontrolü
  - AdminOnly odalarda sadece Admin ve Owner mesaj gönderebilir
- [ ] Online kullanıcı listesi
  - Redis'te online kullanıcıları tut (`online:room:{roomId}` → Set)
  - SSE bağlantısı açılınca kullanıcıyı ekle, kapanınca çıkar
  - `GET /api/room/online/{roomId}` endpoint'i
- [ ] Yazıyor... bildirimi
  - `POST /api/chat/typing/{roomId}` endpoint'i
  - Redis Pub/Sub ile odadaki kullanıcılara ilet
  - SSE event type: `typing`
- [ ] Okudu bildirimi
  - Message entity'sine `ReadBy` veya ayrı `MessageRead` tablosu
  - `POST /api/chat/read/{messageId}` endpoint'i
  - SSE event type: `read`

### Frontend (Electron.js)
- [ ] Proje kurulumu (Electron + React veya vanilla JS)
- [ ] Login / Register ekranı
- [ ] Room listesi ekranı
- [ ] Chat ekranı
  - SSE bağlantısı
  - Mesaj gönderme
  - Mesaj geçmişi yükleme
  - Online kullanıcı listesi
  - Yazıyor... göstergesi
  - Okudu bilgisi

---

## 🗂 Mimari Notlar

| Katman | Sorumluluk |
|---|---|
| Api | Controller'lar, HTTP katmanı |
| Business | Servis interface ve implementasyonları |
| DataAccess | EF Dal'ları, Konfigürasyonlar, Context |
| Entities | Entity'ler, DTO'lar, Enum'lar |
| Core | BaseEntity, IEntity, Generic Repository |

## 🔑 Önemli Konvansiyonlar
- Tüm ID'ler `Guid`
- Redis cache key: `chat:room:{roomId}:messages`
- Redis pub/sub channel: `chatroom:{roomId}`
- Redis online set key: `online:room:{roomId}`
- SSE event format: `data: {payload}\n\n`
- JWT claim key: `ClaimTypes.NameIdentifier` → `user.Id`
