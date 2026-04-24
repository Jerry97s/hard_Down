# HdLabs Labs (모노레포)

**[커밋 기록 · 변경 이력 보기 →](docs/커밋-기록.md)**

---

HdLabs 시리즈 데스크톱 도구 모음입니다. **.NET 9 / WPF** 기반이며 `src/HdLabs.Common`을 공유합니다.

## 솔루션 구성

| 프로젝트 | 설명 | README |
|----------|------|--------|
| **HdLabs.Memo** | 스티키 메모형 메모 앱 (저장·목록·JSON 영속) | [src/HdLabs.Memo/README.md](src/HdLabs.Memo/README.md) |
| **HdLabs.Finder** | 파일 이름 검색 (비동기) | [src/HdLabs.Finder/README.md](src/HdLabs.Finder/README.md) |
| **HdLabs.Zip** | ZIP 유틸 (스캐폴딩) | [src/HdLabs.Zip/README.md](src/HdLabs.Zip/README.md) |
| **HdLabs.Down** | URL 분석·다운로드 (스캐폴딩) | [src/HdLabs.Down/README.md](src/HdLabs.Down/README.md) |
| **HdLabs.Capture** | 화면 캡처 (스캐폴딩) | [src/HdLabs.Capture/README.md](src/HdLabs.Capture/README.md) |
| **HdLabs.Cam** | 웹캠 (스캐폴딩) | [src/HdLabs.Cam/README.md](src/HdLabs.Cam/README.md) |
| **HdLabs.Common** | MVVM·테마 공용 라이브러리 | [src/HdLabs.Common/README.md](src/HdLabs.Common/README.md) |

## 빌드

```bash
dotnet build src/HdLabs.Memo/HdLabs.Memo.csproj -c Release
```

각 앱은 `src/HdLabs.*/HdLabs.*.csproj`에서 동일하게 빌드할 수 있습니다.

## Git 원격 (GitHub)

동일 모노레포를 여러 저장소에 미러할 수 있도록 원격이 나뉘어 있습니다. 자세한 푸시 방법은 [docs/커밋-기록.md](docs/커밋-기록.md)를 참고하세요.
