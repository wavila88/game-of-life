import './App.css'

import { QueryClientProvider, QueryClient} from '@tanstack/react-query'
import GameOfLifeContainer from './features/gameOfLife/components/gameOfLifeContainer/GameOfLifeContainer';
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
