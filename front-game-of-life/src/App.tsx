import './App.css'
import GameOfLifeContainer from './components/GameOfLifeContainer/GameOfLifeContainer'
import { QueryClientProvider, QueryClient} from '@tanstack/react-query'
function App() {
  
const queryClient = new QueryClient();
  return (
    <>
      <QueryClientProvider client={queryClient}>
        <GameOfLifeContainer />
      </QueryClientProvider>
     
    </>
  )
}

export default App
