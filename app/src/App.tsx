import './App.scss';
import * as React from 'react';

function App() {

  var title = "Welcome";

  React.useEffect(() => {
    document.title = `${title} | Outboard`;
  });


  return (
    <div className="app">
      Hello, world!
    </div>
  );
}

export default App;