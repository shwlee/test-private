# Net10Sample.Tests

.NET 10 private 메서드 테스트 샘플입니다. 아래 명령은 솔루션 폴더에서 실행합니다.

기본 Debug 구성의 전체 테스트 (Reflection, UnsafeAccessor, Source Generator, 33개):

```powershell
dotnet test Net10Sample.sln
```

별도의 Test 구성에서도 동일한 33개 테스트를 실행합니다.

```powershell
dotnet test Net10Sample.sln -c Test
```

Visual Studio에서는 Debug 또는 Test 구성으로 빌드한 뒤 테스트 탐색기에서 실행합니다.

Debug와 Test 구성에서 원본 및 테스트 프로젝트에 `TEST_EXPOSE` 상수가 정의됩니다. 생성기는 같은 타입의 partial 선언에 `Test_CalculateDiscountAmount` 래퍼를 추가하므로, 원본 `PriceCalculator`도 partial이어야 합니다.

Release 구성에서는 래퍼가 포함되지 않습니다. Source Generator 테스트와 데이터 선언은 `#if TEST_EXPOSE`로 제외되지만 파일은 프로젝트에 계속 표시됩니다. Release에서는 Reflection 및 UnsafeAccessor 테스트 22개가 실행됩니다.

생성기 프로젝트는 Visual Studio와 CLI 컴파일러 호스트에서 사용할 수 있도록 netstandard2.0을 대상으로 합니다. 원본 및 테스트 프로젝트는 net10.0을 유지합니다.

생성된 래퍼는 private 메서드를 직접 호출하므로 예외가 TargetInvocationException으로 감싸지지 않습니다.
